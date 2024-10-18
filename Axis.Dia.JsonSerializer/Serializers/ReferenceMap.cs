using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;

namespace Axis.Dia.Json.Serializers
{
    internal class ReferenceMap
    {
        private readonly Dictionary<int, (IDiaValue RefValue, int JsonHash)> _valueMap = new();
        private readonly Dictionary<int, int> _hashMap = [];

        private bool TryAddRef(
            IDiaValue @ref,
            int jsonHash,
            out (IDiaValue RefValue, int JsonHash) value)
        {
            // is the json-hash already mapped?
            if (_hashMap.TryGetValue(jsonHash, out var refHash))
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
                _hashMap[jsonHash] = refHash;
                _valueMap[refHash] = value = (@ref, jsonHash);
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
            out (IDiaValue RefValue, int JsonHash) value)
            => TryAddRef(record, _valueMap.Count, out value);

        public bool TryAddRef(
            Sequence sequence,
            out (IDiaValue RefValue, int JsonHash) value)
            => TryAddRef(sequence, _valueMap.Count, out value);
    }
}
