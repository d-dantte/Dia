using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Axon.Parsers
{
    public class ReferenceParser : IValueSerializer<ReferenceValue>
    {
        #region Symbols
        public const string SymbolNameDiaRef = "dia-ref";
        public const string SymbolNameAddressIndex = "address-index";
        #endregion

        public static string GrammarSymbol => SymbolNameDiaRef;

        public static IResult<ReferenceValue> Parse(CSTNode symbolNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);
            context.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(context)} instance"));

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            try
            {
                var (_, AnnotationNode, ValueNode) = symbolNode.DeconstructValueNode();
                var addressIndex = AddressIndexParser.Parse(ValueNode);
                var annotationResult = AnnotationNode is null
                    ? Result.Of(Array.Empty<Annotation>())
                    : AnnotationParser.Parse(AnnotationNode, context);

                return annotationResult
                    .Combine(addressIndex, (annotations, addressIndex) => ReferenceValue.Of(
                        context.Track(addressIndex),
                        annotations));
            }
            catch(Exception e)
            {
                return Result.Of<ReferenceValue>(e);
            }
        }

        public static IResult<string> Serialize(ReferenceValue value, SerializerContext context)
        {
            context.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(context)}: default"));

            if (value.IsNull)
                return Result.Of<string>(new InvalidOperationException("Null dia-reference"));

            if (!value.IsLinked)
                return Result.Of<string>(new InvalidOperationException("Unlinked reference"));

            if (!context.TryGetAddressIndex(value.Value!, out var index))
                return Result.Of<string>(new InvalidOperationException("Reference address missing"));

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);

            return annotationText!.Map(annotation => $"{annotation}@0x{index:x}");
        }
    }
}
