using Axis.Dia.Core;

namespace Axis.Dia.PathQuery
{
    public interface IModify
    {
        IEnumerable<IDiaValue> ReplaceValues(
            IDiaValue root,
            ConditionalValueProvider valueProvider);

        public delegate bool ConditionalValueProvider(
            (object Key, IDiaValue Value) keyValuePair,
            out IDiaValue? value);
    }
}
