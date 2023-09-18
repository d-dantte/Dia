using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Json.Parser
{
    internal static class EncodedValueExtractor
    {
        internal const string SymbolNameEncodedValue = "encoded-value";
        internal const string SymbolNameValueMetadata = "value-metadata";
        internal const string SymbolNameRefIndex = "ref-index";
        internal const string SymbolNameIntDigits = "int-digits";
        internal const string SymbolNameDeclaredType = "declared-type";
        internal const string SymbolNameTypes = "types";
        internal const string SymbolNameAnnotationList = "annotation-list";
        internal const string SymbolNameValueText = "value-content";

        public static IResult<Info> ExtractEncodedInfo(
            CSTNode encodedValueNode,
            DiaType expectedType,
            ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(encodedValueNode);
            ArgumentNullException.ThrowIfNull(context);

            if (!SymbolNameEncodedValue.Equals(encodedValueNode.SymbolName))
                return Result.Of<Info>(new InvalidOperationException(
                    $"Invalid symbol: '{encodedValueNode}', expected '{SymbolNameEncodedValue}'"));

            try
            {
                // declared type
                var type = encodedValueNode
                    .FindNodes($"{SymbolNameValueMetadata}/{SymbolNameDeclaredType}/{SymbolNameTypes}")
                    .Select(node => Enum.Parse<DiaType>(node.TokenValue()))
                    .First();

                if (!expectedType.Equals(type))
                    throw new InvalidOperationException(
                        $"Invalid declared type: '{type}', expected '{expectedType}'");

                // ref index
                var refIndex = encodedValueNode
                    .FindNodes($"{SymbolNameValueMetadata}/{SymbolNameRefIndex}/{SymbolNameIntDigits}")
                    .Select(node => int.Parse(node.TokenValue()).AsNullable())
                    .FirstOrDefault();

                // annotations
                var annotationResult = encodedValueNode
                    .FindNodes($"{SymbolNameValueMetadata}/{SymbolNameAnnotationList}")
                    .First()
                    .ApplyTo(node => AnnotationParser.Parse(node, context));

                // content
                var valueText = encodedValueNode
                    .FindNodes($"{SymbolNameValueText}")
                    .First();


                return annotationResult.Map(annotations => new Info(refIndex, valueText, annotations));
            }
            catch(Exception e)
            {
                return Result.Of<Info>(e);
            }
        }



        public readonly struct Info: IDefaultValueProvider<Info>
        {
            public int? AddressIndex { get; }

            public Annotation[] Annotations { get; }

            public CSTNode ValueText { get; }

            public bool IsDefault => Equals(default);

            static Info IDefaultValueProvider<Info>.Default => default;

            public Info(int? addressIndex, CSTNode valueText, Annotation[] annotations)
            {
                AddressIndex = addressIndex;
                Annotations = annotations ?? throw new ArgumentNullException(nameof(annotations));
                ValueText = valueText ?? throw new ArgumentNullException(nameof(valueText));
            }
        }
    }
}
