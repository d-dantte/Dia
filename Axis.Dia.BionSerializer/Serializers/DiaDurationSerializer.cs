using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Serializers
{
    public class DiaDurationSerializer :
        ITypeSerializer<Core.Types.Duration>,
        IMetadataProvider<Core.Types.Duration>,
        IDefaultInstance<DiaDurationSerializer>
    {
        public static DiaDurationSerializer DefaultInstance { get; } = new();

        public TypeMetadata ExtractMetadata(Core.Types.Duration value)
        {
            return TypeMetadata
                .Of(DiaType.Duration)
                .WithAnnotated(!value.Attributes.IsEmpty)
                .WithNull(value.IsNull)
                .WithCustomBit(!TimeSpan.Zero.Equals(value.Value ?? TimeSpan.Zero));
        }

        public void SerializeType(Core.Types.Duration value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Write the metadata
            this.ExtractMetadata(value)
                .ApplyTo(meta => meta.Metadata.ToByteArray())
                .Consume(array => context.Buffer.Write(array));

            // Write attributes
            context.TypeSerializer.SerializeAttributeSet(value.Attributes, context);

            // Write data
            long nanoSeconds = 0;
            if (value.IsNull || (nanoSeconds = value.NanoSeconds!.Value) == 0)
                return;

            BitConverter
                .GetBytes(nanoSeconds)
                .ApplyTo(ByteChunks.Of)
                .Consume(chunks => context.Buffer.Write(chunks));
        }
    }
}
