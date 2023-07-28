using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using static Axis.Dia.IO.Text.TextSerializerOptions;

namespace Axis.Dia.IO.Text.Parsers
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

        public static IResult<BlobValue> Parse(CSTNode symbolNode, TextSerializerContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            try
            {
                context ??= new TextSerializerContext();

                var (AnnotationNode, ValueNode) = symbolNode.DeconstructValue();
                var annotationResult = AnnotationNode is null
                    ? Result.Of(Array.Empty<Annotation>())
                    : AnnotationParser.Parse(AnnotationNode, context);

                return ValueNode.SymbolName switch
                {
                    SymbolNameNullBlob => annotationResult.Map(BlobValue.Null),

                    SymbolNameBlobValue => ValueNode
                        .TokenValue()
                        .UnwrapFrom("<", ">")
                        .Trim()
                        .ApplyTo(Convert.FromBase64String)
                        .ApplyTo(bytes => BlobValue.Of(bytes, annotationResult.Resolve()))
                        .ApplyTo(Result.Of),

                    _ => Result.Of<BlobValue>(new ArgumentException(
                        $"Invalid symbol encountered: '{ValueNode.SymbolName}'. "
                        + $"Expected '{SymbolNameNullBlob}', or '{SymbolNameBlobValue}'"))
                };
            }
            catch (Exception e)
            {
                return Result.Of<BlobValue>(e);
            }
        }


        public static IResult<string> Serialize(BlobValue value, TextSerializerContext? context = null)
        {
            context ??= new TextSerializerContext();
            var blobOptions = context.Options.Blobs;

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);
            var valueText = value.Value switch
            {
                null => Result.Of("null.blob"),
                _ => Result.Of(() => blobOptions.LineStyle switch
                {
                    TextLineStyle.Singleline => Convert
                        .ToBase64String(value.Value!)
                        .WrapIn("< ", " >"),

                    TextLineStyle.Multiline => Convert
                        .ToBase64String(value.Value!)
                        .Batch(blobOptions.MaxLineLength)
                        .Select(chars => new string(chars.ToArray()))
                        .ApplyTo(lines => WrapLines(lines.ToArray(), context)),
                    _ => throw new ArgumentException($"Invalid line style: {blobOptions.LineStyle}")
                })
            };

            return annotationText!.Combine(valueText, (ann, value) => $"{ann}{value}");
        }

        internal static string WrapLines(string[] lines, TextSerializerContext context)
        {
            return lines.Length switch
            {
                0 => "< >",
                1 => $"< {lines[0]} >",
                _ => lines
                    .Prepend("")
                    .JoinUsing($"{Environment.NewLine}{context.Indentation(1)}")
                    .WrapIn("<", $"{Environment.NewLine}>")
            };
        }
    }
}
