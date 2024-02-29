using System.Runtime.ExceptionServices;

namespace Axis.Dia.Core.Utils
{
    internal static class ExceptionExtensions
    {
        internal static T ThrowIf<T>(this
            T value,
            Func<T, bool> predicate,
            Func<T, Exception> exceptionMapper)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(exceptionMapper);

            if (predicate.Invoke(value))
                ExceptionDispatchInfo
                    .Capture(exceptionMapper.Invoke(value))
                    .Throw();

            return value;
        }

        internal static T ThrowIfNot<T>(this
            T value,
            Func<T, bool> predicate,
            Func<T, Exception> exceptionMapper)
            => value.ThrowIf(v => !predicate.Invoke(v), exceptionMapper);

        internal static T ThrowIfNull<T>(this
            T value,
            Func<Exception> exceptionMapper)
            => value.ThrowIf(v => v is null, _ => exceptionMapper.Invoke());

        internal static IEnumerable<T> ThrowIfAny<T>(this
            IEnumerable<T> items,
            Func<T, bool> predicate,
            Func<T, Exception> exceptionMapper)
        {
            ArgumentNullException.ThrowIfNull(items);
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(exceptionMapper);

            foreach(var item in items)
            {
                if (predicate.Invoke(item))
                    ExceptionDispatchInfo
                        .Capture(exceptionMapper.Invoke(item))
                        .Throw();

                yield return item;
            }
        }

        internal static void ForAll<T>(this
            IEnumerable<T> items,
            Action<T> consumer)
        {
            ArgumentNullException.ThrowIfNull(items);
            ArgumentNullException.ThrowIfNull(consumer);

            foreach (var item in items)
            {
                consumer.Invoke(item);
            }
        }
    }
}
