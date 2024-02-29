namespace Axis.Dia.Core
{
    public interface IValueEquatable<TSelf>
        where TSelf : IValueEquatable<TSelf>
    {
        bool ValueEquals(TSelf other);

        int ValueHash();
    }
}
