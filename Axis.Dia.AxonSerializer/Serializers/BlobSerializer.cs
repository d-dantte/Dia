using Axis.Dia.Core;
using Axis.Luna.Extensions;

namespace Axis.Dia.Axon.Serializers
{
    public class BlobSerializer : IValueSerializer<Core.Types.Blob>
    {
        private static readonly string NullTypeText = $"null.{DiaType.Blob.ToString().ToLower()}";

        public static string Serialize(Core.Types.Blob value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)} "
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            var base64 = Convert.ToBase64String(value.Value!.Value.AsSpan());

            if (string.Empty.Equals(base64))
                return $"'{DiaType.Blob} '";

            var blobText = context.Options.Indentation switch
            {
                Options.IndentationStyle.None => $"'{DiaType.Blob} {base64}'",                
                _ => base64
                    .BatchGroup(context.Options.Blobs.SingleLineCharacterCount)
                    .Select(tuple => tuple.BatchIndex switch
                    {
                        0 => $"'{DiaType.Blob} {tuple.Batch.JoinUsing()}'",
                        _ => $"{context.Options.NewLine}{context.Indent()}+ '{tuple.Batch.JoinUsing()}'"
                    })
                    .JoinUsing("")
            };

            return $"{attributeText}{blobText}";
        }
    }
}
