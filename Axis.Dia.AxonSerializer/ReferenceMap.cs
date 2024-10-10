using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;

namespace Axis.Dia.AxonSerializer
{
    public class ReferenceMap
    {
        private readonly Dictionary<int, (IDiaValue RefValue, int AxonHash)> _valueMap = new();
        private readonly Dictionary<int, int> _hashMap = new();

        private bool TryAddRef(
            IDiaValue @ref,
            int axonHash,
            out (IDiaValue RefValue, int AxonHash) value)
        {
            // is the axon-hash already mapped?
            if (_hashMap.TryGetValue(axonHash, out var refHash))
            {
                value = _valueMap[refHash];
                return false;
            }

            refHash = GetRefHash(@ref);

            // has the ref instance already mapped?
            if (_valueMap.TryGetValue(refHash, out value))
                return false;

            else
            {
                _hashMap[axonHash] = refHash;
                _valueMap[refHash] = value = (@ref, axonHash);
                return true;
            }
        }

        private static int GetRefHash(IDiaValue @ref)
        {
            return @ref switch
            {
                Record r => r.RefHash(),
                Symbol s => s.RefHash(),
                Sequence s => s.RefHash(),
                Core.Types.String s => s.RefHash(),
                _ => throw new ArgumentException($"Invalid ref type: {@ref?.GetType()}")
            };
        }

        public bool TryAddRef(
            Record record,
            out (IDiaValue RefValue, int AxonHash) value)
            => TryAddRef(record, _valueMap.Count, out value);

        public bool TryAddRef(
            Record record,
            int axonHash)
            => TryAddRef(record, axonHash, out _);

        public bool TryAddRef(
            Sequence sequence,
            out (IDiaValue RefValue, int AxonHash) value)
            => TryAddRef(sequence, _valueMap.Count, out value);

        public bool TryAddRef(
            Sequence sequence,
            int axonHash)
            => TryAddRef(sequence, axonHash, out _);

        public bool TryGetRef(int axonHash, out IDiaValue? refValue)
        {
            refValue = default;

            if (_hashMap.TryGetValue(axonHash, out var refHash)
                && _valueMap.TryGetValue(refHash, out var payload))
            {
                refValue = payload.RefValue;
                return true;
            }

            return false;
        }
    }
}
