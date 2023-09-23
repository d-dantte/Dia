using Axis.Dia.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Json.Parser
{
    internal class ContainerMetadataParser :
        IRootSymbolProvider,
        IJsonConverter<ContainerMetadataParser.ContainerMetadata>
    {
        internal const string SymbolNameContainerMetadata = "container-metadata";
        internal const string SymbolNameRefIndex = "ref-index";
        internal const string SymbolNameHexInt = "hex-int";
        internal const string SymbolNameAnnotationList = "annotation-list";

        public static string RootSymbol => SymbolNameContainerMetadata;

        public static IResult<ContainerMetadata> Parse(CSTNode valueMetadataNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(valueMetadataNode);
            context.ThrowIfDefault(new ArgumentException($"Invalid {nameof(context)} instance"));

            if (!SymbolNameContainerMetadata.Equals(valueMetadataNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol: '{valueMetadataNode}', expected '{SymbolNameContainerMetadata}'");

            try
            {
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

                return annotationResult.Map(annotations => new ContainerMetadata(addressIndex, annotations));
            }
            catch (Exception e)
            {
                return Result.Of<ContainerMetadata>(e);
            }
        }

        public static IResult<string> Serialize(ContainerMetadata value, SerializerContext context)
        {
            value.ThrowIfDefault(new ArgumentException($"Invalid {nameof(value)} instance"));
            context.ThrowIfDefault(new ArgumentException($"Invalid {nameof(context)} instance"));

            var addressIndex = value.AddressIndex is not null
                ? $"#0x{value.AddressIndex.Value:x};"
                : "";

            var annotationsResult = AnnotationParser.Serialize(value.Annotations, context);

            return annotationsResult.Map(annotations => $"[{addressIndex}{annotations}]");
        }


        #region Nested types
        internal readonly struct ContainerMetadata
        {
            public int? AddressIndex { get; }

            public Annotation[] Annotations { get; }

            public ContainerMetadata(int? addressIndex, Annotation[] annotations)
            {
                AddressIndex = addressIndex;
                Annotations = annotations ?? throw new ArgumentNullException(nameof(annotations));
            }
        }
        #endregion
    }
}
