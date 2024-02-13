using System.Runtime.ExceptionServices;

namespace Axis.Dia.Core.Utils
{
    public static class ExceptionExtensions
    {
        public static T ThrowIf<T>(this
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

        public static T ThrowIfNot<T>(this
            T value,
            Func<T, bool> predicate,
            Func<T, Exception> exceptionMapper)
            => value.ThrowIf(v => !predicate.Invoke(v), exceptionMapper);

        public static T ThrowIfNull<T>(this
            T value,
            Func<T, Exception> exceptionMapper)
            => value.ThrowIf(v => v is null, exceptionMapper);
    }
}
