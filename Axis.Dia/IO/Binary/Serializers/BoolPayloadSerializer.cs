using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.IO.Binary.Serializers
{
    internal class BoolPayloadSerializer :
        IPayloadSerializer<BoolValue>,
        IPayloadProvider<BoolValue>
    {
        // prohibits instantiation
        private BoolPayloadSerializer() { }

        public static ValuePayload<BoolValue> CreatePayload(BoolValue value)
        {
            TypeMetadata.MetadataFlags metadataFlags =
                (!value.Annotations.IsEmpty() ? TypeMetadata.MetadataFlags.Annotated : TypeMetadata.MetadataFlags.None)
                | (value.IsNull ? TypeMetadata.MetadataFlags.Null : TypeMetadata.MetadataFlags.None)
                | ((value.Value ?? false) ? TypeMetadata.MetadataFlags.Custom : TypeMetadata.MetadataFlags.None);

            return new ValuePayload<BoolValue>(
                typeMetadata: TypeMetadata.Of(value.Type, metadataFlags),
                value: value);
        }

        public static IResult<BoolValue> Deserialize(
            Stream stream,
            TypeMetadata typeMetadata,
            BinarySerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Bool.Equals(typeMetadata.Type))
                throw new ArgumentException($"Invalid TypeMetadata.Type: '{typeMetadata.Type}', expected: '{DiaType.Bool}'");

            return Result
                .Of(typeMetadata)

                // read the annotations
                .Map(tmeta => (
                    TypeMetadata: tmeta,
                    Annotations: tmeta.IsAnnotated
                        ? AnnotationSerializer.Deserialize(stream, context).Resolve()
                        : Array.Empty<Annotation>()))

                // construct the BoolValue
                .Map(meta => BoolValue.Of(
                    meta.TypeMetadata.IsNull ? null : meta.TypeMetadata.IsCustomFlagSet,
                    meta.Annotations));
        }

        public static IResult<byte[]> Serialize(
            BoolValue value,
            BinarySerializerContext context)
        {
            try
            {
                var payload = CreatePayload(value);
                return AnnotationSerializer
                    .Serialize(value.Annotations, context)
                    .Map(annotationBytes => payload.TypeMetadata.Metadata
                        .ToArray()
                        .JoinWith(annotationBytes));
            }
            catch(Exception e)
            {
                return Result.Of<byte[]>(e);
            }
        }
    }
}
