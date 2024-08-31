using Axis.Dia.Core;

namespace Axis.Dia.PathQuery
{
    public interface IDelete
    {
        IEnumerable<IDiaValue> DeleteValues(IDiaValue root);
    }
}
