using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Json.Parser
{
    internal class ValueMetadataParser :
        IRootSymbolProvider,
        IJsonConverter<ValueMetadataParser.ValueMetadata>
    {
        internal const string SymbolNameValueMetadata = "value-metadata";
        internal const string SymbolNameRefIndex = "ref-index";
        internal const string SymbolNameHexInt = "hex-int";
        internal const string SymbolNameDeclaredType = "declared-type";
        internal const string SymbolNameTypes = "types";
        internal const string SymbolNameAnnotationList = "annotation-list";

        public static string RootSymbol => SymbolNameValueMetadata;

        public static IResult<ValueMetadata> Parse(CSTNode valueMetadataNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(valueMetadataNode);
            context.ThrowIfDefault(new ArgumentException($"Invalid {nameof(context)} instance"));

            if (!SymbolNameValueMetadata.Equals(valueMetadataNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol: '{valueMetadataNode}', expected '{SymbolNameValueMetadata}'");

            try
            {
                // declared type
                var type = valueMetadataNode
                    .FindNodes($"{SymbolNameDeclaredType}/{SymbolNameTypes}")
                    .Select(node => Enum.Parse<DiaType>(node.TokenValue()))
                    .First();

                // ref index
                var addressIndex = valueMetadataNode
                    .FindNodes($"{SymbolNameRefIndex}/{SymbolNameHexInt}")
                    .Select(node => int
                        .Parse(
                            node.TokenValue()[2..], // remove '0x'
                            System.Globalization.NumberStyles.HexNumber)
                        .AsNullable())
                    .FirstOrDefault();

                // annotations
                var annotationResult = valueMetadataNode
                    .FindNodes($"{SymbolNameAnnotationList}")
                    .FirstOrOptional()
                    .Map(node => AnnotationParser.Parse(node, context))
                    .ValueOr(Result.Of(Array.Empty<Annotation>()));

                return annotationResult.Map(annotations => new ValueMetadata(addressIndex, type, annotations));
            }
            catch (Exception e)
            {
                return Result.Of<ValueMetadata>(e);
            }
        }

        public static IResult<string> Serialize(ValueMetadata value, SerializerContext context)
        {
            value.ThrowIfDefault(new ArgumentException($"Invalid {nameof(value)} instance"));
            context.ThrowIfDefault(new ArgumentException($"Invalid {nameof(context)} instance"));

            var addressIndex = value.AddressIndex is not null
                ? $"#0x{value.AddressIndex.Value:x};"
                : "";

            var typeDeclaration = $"${value.Type};";

            var annotationsResult = AnnotationParser.Serialize(value.Annotations, context);

            return annotationsResult.Map(annotations => $"[{addressIndex}{typeDeclaration}{annotations}]");
        }


        #region Nested types
        internal readonly struct ValueMetadata
        {
            public int? AddressIndex { get; }

            public Annotation[] Annotations { get; }

            public DiaType Type { get; }

            public ValueMetadata(int? addressIndex, DiaType type, Annotation[] annotations)
            {
                AddressIndex = addressIndex;
                Type = type;
                Annotations = annotations ?? throw new ArgumentNullException(nameof(annotations));
            }

            public static ValueMetadata Of(IDiaValue value, int? addressIndex)
            {
                ArgumentNullException.ThrowIfNull(value);

                return new ValueMetadata(addressIndex, value.Type, value.Annotations);
            }
        }
        #endregion
    }
}
