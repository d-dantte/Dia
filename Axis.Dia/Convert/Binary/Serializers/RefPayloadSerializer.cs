using Axis.Dia.Contracts;
using Axis.Dia.Convert.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Convert.Binary.Serializers
{
    internal class RefPayloadSerializer :
        IPayloadSerializer<ReferenceValue>
    {
        // prohibits instantiation
        private RefPayloadSerializer() { }

        public static IResult<ReferenceValue> Deserialize(
            Stream stream,
            TypeMetadata typeMetadata,
            DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Ref.Equals(typeMetadata.Type))
                throw new ArgumentException($"Invalid TypeMetadata.Type: '{typeMetadata.Type}', expected: '{DiaType.Ref}'");

            return Result
                .Of(typeMetadata)

                // read the annotations
                .Map(tmeta => (
                    RefIndex: tmeta.CustomMetadata.ToBigInteger(),
                    Annotations: tmeta.IsAnnotated
                        ? AnnotationSerializer.Deserialize(stream).Resolve()
                        : Array.Empty<Annotation>()))

                // construct the ReferenceValue
                .Map(meta => ReferenceValue.Of(
                    !context.TryGetRefAddress(meta.RefIndex, out var address)
                        ? throw new InvalidOperationException($"Reference with index '{meta.RefIndex}' has not been allocated")
                        : address,
                    meta.Annotations))

                // if the value could not be deserialized, creates an instance of ValueDeserializationException
                .MapError(PayloadSerializer.TranslateValueError<ReferenceValue>);
        }

        public static IResult<byte[]> Serialize(ReferenceValue value, SerializerContext context)
        {
            try
            {
                var flags =
                    (value.HasAnnotations() ? TypeMetadata.MetadataFlags.Annotated : TypeMetadata.MetadataFlags.None)
                    | (value.IsNull ? TypeMetadata.MetadataFlags.Null : TypeMetadata.MetadataFlags.None);

                var tmeta = TypeMetadata.Of(DiaType.Ref, flags, context.CurrentIndex);

                return AnnotationSerializer
                    .Serialize(value.Annotations)
                    .Map(annotationBytes => tmeta.Metadata.ToArray().JoinWith(annotationBytes));
            }
            catch(Exception e)
            {
                return Result.Of<byte[]>(e);
            }
        }
    }
}
