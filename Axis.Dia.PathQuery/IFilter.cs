using Axis.Dia.Core.Contracts;

namespace Axis.Dia.PathQuery
{
    public interface IFilter
    {
        IEnumerable<IDiaValue> FilterValues(IDiaValue root);
    }
}
