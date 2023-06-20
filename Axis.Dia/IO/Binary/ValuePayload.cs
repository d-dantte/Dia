using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary.Metadata;

namespace Axis.Dia.IO.Binary
{

    public record ValuePayload<TDiaValue>
    where TDiaValue: IDiaValue
    {
        public TDiaValue Value { get; }

        public TypeMetadata TypeMetadata { get; }

        internal ValuePayload(TDiaValue value, TypeMetadata typeMetadata)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            TypeMetadata = typeMetadata;
        }
    }
}
