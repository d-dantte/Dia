using Axis.Dia.Core.Contracts;

namespace Axis.Dia.PathQuery
{
    public interface IDelete
    {
        IEnumerable<IDiaValue> DeleteValues(IDiaValue root);
    }
}
