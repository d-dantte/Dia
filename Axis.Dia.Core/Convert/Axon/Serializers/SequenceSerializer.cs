using Axis.Dia.Core.Types;
using Axis.Dia.Core.Utils;

namespace Axis.Dia.Core.Convert.Axon.Serializers
{
    public class SequenceSerializer : ISerializer<Sequence>
    {
        private static readonly string NullTypeText = $"*.{DiaType.Sequence.ToString().ToLower()}";

        public static string Serialize(Sequence value, SerializerContext context)
        {
            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)}::"
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            if (value.IsEmpty)
                return $"{attributeText}[]";

            var next = context.Next();
            var items = value.Select(v => ValueSerializer.Serialize(v, next));

            var itemsText = !context.Options.Sequences.UseMultiline
                ? items.JoinUsing(", ")
                : items
                    .Select((item, index) => $"{(index > 0 ? ",":"")}{context.Options.NewLine}{next.Indent()}{item}")
                    .JoinUsing()
                    .ApplyTo(txt => $"{txt}{context.Options.NewLine}{context.Indent()}");

            return $"{attributeText}[{itemsText}]";
        }
    }
}
