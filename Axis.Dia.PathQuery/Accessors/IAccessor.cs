namespace Axis.Dia.PathQuery.Accessors
{
    public interface IAccessor
    { }

    public interface IAccessor<TSource, TKey>: IAccessor
    {
        TSource Source { get; }

        TKey Key { get; }
    }
}
