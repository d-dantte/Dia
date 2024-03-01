using Axis.Dia.Core.Types;
using Axis.Dia.Core.Utils;

namespace Axis.Dia.Core.Convert.Axon.Serializers
{
    public class SymbolSerializer : ISerializer<Symbol>
    {
        private static readonly string NullTypeText = $"*.{DiaType.Symbol.ToString().ToLower()}";

        public static string Serialize(Symbol value, SerializerContext context)
        {
            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)}::"
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            var stringText = context.Options.Symbols.UseMultiline switch
            {
                false => value.Value!
                    .Select(EscapeInlineStringCharacter)
                    .JoinUsing()
                    .ApplyTo(s => $"'#{DiaType.Symbol} {s}'"),

                true => value.Value!
                    .BatchGroup(context.Options.Symbols.MultilineThreshold)
                    .Select(batch => (LineIndex: batch.BatchIndex, Line: batch.Batch
                        .Select(EscapeInlineStringCharacter)
                        .JoinUsing()))
                    .Select(lines => lines.LineIndex switch
                    {
                        0 => $"'#{DiaType.Symbol} {lines.Line}'",
                        _ => $"{context.Options.NewLine}{context.Indent()}+ '{lines.Line}'"
                    })
                    .JoinUsing()
            };

            return $"{attributeText}{stringText}";
        }

        private static string EscapeInlineStringCharacter(char character)
        {
            return character switch
            {
                '\\' => "\\\\",
                '\"' => "\\\"",
                '\n' => "\\\n",
                '\r' => "\\\r",
                '\0' => "\\\0",
                '\a' => "\\\a",
                '\b' => "\\\b",
                '\f' => "\\\f",
                '\t' => "\\\t",
                '\v' => "\\\v",
                _ => (char.IsControl(character), character < 256) switch
                {
                    (true, true) => $"\\x{(int)character:x2}",
                    (true, false) => $"\\u{(int)character:x4}",
                    (_, _) => character.ToString()
                }
            };
        }
    }
}
