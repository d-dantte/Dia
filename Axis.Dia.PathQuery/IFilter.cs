using Axis.Dia.Core;

namespace Axis.Dia.PathQuery
{
    public interface IFilter
    {
        IEnumerable<IDiaValue> FilterValues(IDiaValue root);
    }
}
