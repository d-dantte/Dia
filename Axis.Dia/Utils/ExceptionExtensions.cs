namespace Axis.Dia.Utils
{
    public static class ExceptionExtensions
    {
        public static TValue ThrowIf<TValue>(
            this TValue value,
            Func<TValue, bool> predicate,
            Func<TValue, Exception> exceptionProvider)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(predicate);

            if (predicate.Invoke(value))
                throw exceptionProvider.Invoke(value);

            return value;
        }

        public static TValue ThrowIfNot<TValue>(
            this TValue value,
            Func<TValue, bool> predicate,
            Func<TValue, Exception> exceptionProvider)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(predicate);

            if (!predicate.Invoke(value))
                throw exceptionProvider.Invoke(value);

            return value;
        }

        public static IEnumerable<TItem> ThrowIfAny<TItem>(
            this IEnumerable<TItem> enumerable,
            Func<TItem, bool> predicate,
            Func<IEnumerable<TItem>, Exception> exceptionProvider)
        {
            ArgumentNullException.ThrowIfNull(enumerable);
            ArgumentNullException.ThrowIfNull(predicate);

            if (enumerable.Any(predicate))
                throw exceptionProvider.Invoke(enumerable);

            return enumerable;
        }

        public static IEnumerable<TItem> ThrowIfNone<TItem>(
            this IEnumerable<TItem> enumerable,
            Func<TItem, bool> predicate,
            Func<IEnumerable<TItem>, Exception> exceptionProvider)
        {
            ArgumentNullException.ThrowIfNull(enumerable);
            ArgumentNullException.ThrowIfNull(predicate);

            if (!enumerable.Any(predicate))
                throw exceptionProvider.Invoke(enumerable);

            return enumerable;
        }

        public static IEnumerable<TItem> ThrowIfAll<TItem>(
            this IEnumerable<TItem> enumerable,
            Func<TItem, bool> predicate,
            Func<IEnumerable<TItem>, Exception> exceptionProvider)
        {
            ArgumentNullException.ThrowIfNull(enumerable);
            ArgumentNullException.ThrowIfNull(predicate);

            if (enumerable.All(predicate))
                throw exceptionProvider.Invoke(enumerable);

            return enumerable;
        }
    }
}
