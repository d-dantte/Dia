﻿using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using static Axis.Dia.Convert.Axon.SerializerOptions;

namespace Axis.Dia.Convert.Axon.Parsers
{
    public class BlobParser : IValueSerializer<BlobValue>
    {
        #region Symbols
        public const string SymbolNameDiaBlob = "dia-blob";
        public const string SymbolNameNullBlob = "null-blob";
        public const string SymbolNameBlobValue = "blob-text-value";
        #endregion

        private BlobParser() { }

        public static string GrammarSymbol => SymbolNameDiaBlob;

        public static IResult<BlobValue> Parse(CSTNode symbolNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);
            context.ThrowIfDefault($"Invalid {nameof(context)} instance");

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            try
            {
                var (AddressIndexNode, AnnotationNode, ValueNode) = symbolNode.DeconstructValueNode();
                var annotationResult = AnnotationNode is null
                    ? Result.Of(Array.Empty<Annotation>())
                    : AnnotationParser.Parse(AnnotationNode, context);

                var result = ValueNode.SymbolName switch
                {
                    SymbolNameNullBlob => annotationResult.Map(BlobValue.Null),

                    SymbolNameBlobValue => ValueNode
                        .TokenValue()
                        .UnwrapFrom("<", ">")
                        .Replace(" ", "")
                        .Replace("\n", "")
                        .Replace("\r", "")
                        .Replace("\t", "")
                        .ApplyTo(Result.Of<string>)
                        .Map(System.Convert.FromBase64String)
                        .Map(bytes => BlobValue.Of(bytes, annotationResult.Resolve())),

                    _ => Result.Of<BlobValue>(new ArgumentException(
                        $"Invalid symbol encountered: '{ValueNode.SymbolName}'. "
                        + $"Expected '{SymbolNameNullBlob}', or '{SymbolNameBlobValue}'"))
                };

                return AddressIndexNode is not null
                    ? result.Combine(
                        AddressIndexParser.Parse(AddressIndexNode),
                        (value, addressIndex) => value.RelocateValue(context.Track(addressIndex)))
                    : result;
            }
            catch (Exception e)
            {
                return Result.Of<BlobValue>(e);
            }
        }


        public static IResult<string> Serialize(BlobValue value, SerializerContext context)
        {
            context.ThrowIfDefault($"Invalid {nameof(context)} instance");

            var blobOptions = context.Options.Blobs;

            var addressIndexText = context.TryGetAddressIndex(value, out var index)
                ? $"#0x{index:x}"
                : "";

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);
            var valueText = value.Value switch
            {
                null => Result.Of("null.blob"),
                _ => Result.Of(() => blobOptions.LineStyle switch
                {
                    TextLineStyle.Singleline => System.Convert
                        .ToBase64String(value.Value!)
                        .WrapIn("< ", " >"),

                    TextLineStyle.Multiline => System.Convert
                        .ToBase64String(value.Value!)
                        .Batch(blobOptions.MaxLineLength)
                        .Select(chars => new string(chars.ToArray()))
                        .ApplyTo(lines => WrapLines(lines.ToArray(), context)),
                    _ => throw new ArgumentException($"Invalid line style: {blobOptions.LineStyle}")
                })
            };

            return annotationText!.Combine(valueText, (ann, value) => $"{addressIndexText}{ann}{value}");
        }

        internal static string WrapLines(string[] lines, SerializerContext context)
        {
            return lines.Length switch
            {
                0 => "< >",
                1 => $"< {lines[0]} >",
                _ => lines
                    .Prepend("")
                    .JoinUsing($"{Environment.NewLine}{context.IndentText("", 1)}")
                    .WrapIn("<", $"{Environment.NewLine}>")
            };
        }
    }
}
