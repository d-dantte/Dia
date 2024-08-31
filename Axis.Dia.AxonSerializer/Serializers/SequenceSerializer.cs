using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.Axon.Serializers
{
    public class SequenceSerializer : IValueSerializer<Sequence>
    {
        private static readonly string NullTypeText = $"null.{DiaType.Sequence.ToString().ToLower()}";

        public static string Serialize(Sequence value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)} "
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            else if (context.ReferenceMap.TryAddRef(value, out var refInfo))
            {
                var axonHashText = $"#{refInfo.AxonHash:x}; ";

                if (value.IsEmpty)
                    return $"{attributeText}[]";

                var next = context.Next();
                var items = value.Select(v => ValueSerializer.Serialize(v, next));

                var itemsText = !context.Options.Sequences.UseMultiline
                    ? items.JoinUsing(", ")
                    : items
                        .Select((item, index) => $"{(index > 0 ? "," : "")}{context.Options.NewLine}{next.Indent()}{item}")
                        .JoinUsing("")
                        .ApplyTo(txt => $"{txt}{context.Options.NewLine}{context.Indent()}");

                return $"{axonHashText}{attributeText}[{itemsText}]";
            }
            else return $"'Ref:{value.Type} 0x{refInfo.AxonHash:x}'";
        }

        //TODO: Fix recursive equality tests. E.g, a sequence that references itself, calling equals will get into an infinite loop. Same with records.
    }
}
