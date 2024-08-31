using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.Axon.Serializers
{
    public class SymbolSerializer : IValueSerializer<Symbol>
    {
        private static readonly string NullTypeText = $"null.{DiaType.Symbol.ToString().ToLower()}";

        public static string Serialize(Symbol value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)} "
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            var symbolText = SerializeCanonicalForm(value.Value!, context);

            return $"{attributeText}{symbolText}";
        }

        private static string SerializeCanonicalForm(string value, SerializerContext context)
        {
            return context.Options.Symbols.LineThreshold switch
            {
                null => value
                    .EscapeUnicodeControlCharacters()
                    .ApplyTo(s => $"'{DiaType.Symbol} {s}'"),

                ushort threshold => value
                    .Batch(threshold)
                    .Select(ToString)
                    .Select(EscapeBSol)
                    .Select(EscapeQuote)
                    .Select(CommonExtensions.EscapeUnicodeControlCharacters)
                    .Select((line, index) => index switch
                    {
                        0 => $"'{DiaType.Symbol} {line}'",
                        _ => $"{context.Options.NewLine}{context.Indent()}+ '{line}'"
                    })
                    .JoinUsing("")
            };
        }

        private static string EscapeBSol(
            string value)
            => value?.Replace("\\", "\\\\")!;

        private static string EscapeQuote(
            string value)
            => value?.Replace("'", "\\'")!;

        private static string ToString(
            IEnumerable<char> chars)
            => new(chars.ToArray());
    }
}
