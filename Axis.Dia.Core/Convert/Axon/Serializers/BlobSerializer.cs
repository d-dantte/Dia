using Axis.Dia.Core.Utils;
using Axis.Luna.Extensions;

namespace Axis.Dia.Core.Convert.Axon.Serializers
{
    public class BlobSerializer : ISerializer<Types.Blob>
    {
        private static readonly string NullTypeText = $"*.{DiaType.Blob.ToString().ToLower()}";

        public static string Serialize(Types.Blob value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)}::"
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            var base64 = System.Convert.ToBase64String(value.Value!.Value.AsSpan());
            return (context.Options.Indentation, context.Options.Blobs.UseCanonicalForm) switch
            {
                (Options.IndentationStyle.None, true) => $"{attributeText}'#{DiaType.Blob} {base64}'",
                (Options.IndentationStyle.None, false) => $"{attributeText}<{base64}>",
                (_, true) => base64
                    .BatchGroup(context.Options.Blobs.SingleLineCharacterCount)
                    .Select(tuple => tuple.BatchIndex switch
                    {
                        0 => $"'#{DiaType.Blob} {tuple.Batch.JoinUsing()}'",
                        _ => $"{context.Options.NewLine}{context.Indent()}+ '{tuple.Batch.JoinUsing()}'"
                    })
                    .JoinUsing(),
                (_, false) => base64
                    .BatchGroup(context.Options.Blobs.SingleLineCharacterCount)
                    .Select(tuple => tuple.BatchIndex switch
                    {
                        0 => $"<{tuple.Batch.JoinUsing()}>",
                        _ => $"{context.Options.NewLine}{context.Indent()}+ <{tuple.Batch.JoinUsing()}>"
                    })
                    .JoinUsing()
            };
        }
    }
}
