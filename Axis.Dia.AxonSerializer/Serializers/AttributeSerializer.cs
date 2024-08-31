using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.Axon.Serializers
{
    public class AttributeSerializer
    {
        public static string Serialize(
            AttributeSet attributes,
            SerializerContext context)
        {
            if (attributes.IsDefault)
                throw new ArgumentException("Invalid attributes: default");

            if (context.IsDefault)
                throw new ArgumentException("Invalid context: default");

            if (attributes.IsEmpty)
                return "";

            var serializedAttributes = attributes
                .OrderBy(att => att.Key)
                .Select(Serialize);
            return context.Options.Indentation switch
            {
                Options.IndentationStyle.None => serializedAttributes
                    .JoinUsing(" ")
                    .ApplyTo(s => $"{s}"),

                _ => serializedAttributes
                    .BatchGroup(context.Options.Attributes.SingleLineAttributeCount)
                    .Select(tuple => tuple.BatchIndex switch
                    {
                        0 => $"{tuple.Batch.JoinUsing(" ")}",
                        _ => $"{context.Options.NewLine}{context.Indent()}{tuple.Batch.JoinUsing(" ")}"
                    })
                    .JoinUsing("")
            };
        }

        public static string Serialize(Core.Types.Attribute attribute)
        {
            if (attribute.IsDefault)
                throw new ArgumentException("Invalid attribute: default");

            var valueText = attribute.HasValue
                ? attribute.Value!
                    .ApplyTo(EscapeAttributeValue)
                    .ApplyTo(CommonExtensions.EscapeUnicodeControlCharacters)
                    .ApplyTo(s => $":{s}")
                : "";

            return $"@{attribute.Key}{valueText};";
        }

        internal static string EscapeAttributeValue(string @string)
        {
            return @string
                .Select(c => c switch
                {
                    ':' => "\\:",
                    ';' => "\\;",
                    '\'' => "\\\'",
                    '\\' => "\\\\",
                    _ => c.ToString()
                })
                .JoinUsing("");
        }
    }
}
