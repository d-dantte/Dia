namespace Axis.Dia.Json
{
    internal static class Extensions
    {
        internal static TOut Aggregate<TItem, TOut>(
            this IEnumerable<TItem> items,
            TOut seed,
            Func<TOut, TItem, int, TOut> aggregator)
        {
            ArgumentNullException.ThrowIfNull(items);
            ArgumentNullException.ThrowIfNull(seed);
            ArgumentNullException.ThrowIfNull(aggregator);

            return items
                .Select((Item, Index) => (Item, Index))
                .Aggregate(seed, (accumulator, item) => aggregator.Invoke(accumulator, item.Item, item.Index));
        }
    }
}
