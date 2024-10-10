using System.Text.RegularExpressions;

namespace Axis.Dia.Core.Utils
{
    internal static class CommonExtensions
    {
        //private static readonly Regex UnicodeControlCharacterPattern = new Regex("\\p{C}", RegexOptions.Compiled);
        //internal static string EscapeUnicodeControlCharacter(this char c)
        //{
        //    return c switch
        //    {
        //        '\0' => "\\0",
        //        '\a' => "\\a",
        //        '\b' => "\\b",
        //        '\f' => "\\f",
        //        '\n' => "\\n",
        //        '\r' => "\\r",
        //        '\t' => "\\t",
        //        '\v' => "\\v",
        //        _ => (UnicodeControlCharacterPattern.IsMatch(c.ToString()), c <= byte.MaxValue) switch
        //        {
        //            (true, true) => $"\\x{(int)c:x2}",
        //            (true, false) => $"\\u{(int)c:x4}",
        //            (_, _) => c.ToString()
        //        }
        //    };
        //}

        /// <summary>
        /// <seealso cref="RecursionGuard{TResult, TInstance}(AsyncLocal{HashSet{TInstance}}, TInstance, Func{TInstance, TResult}, Func{TInstance, TResult})"/>
        /// </summary>
        /// <typeparam name="TResult">The result-type of the function</typeparam>
        /// <typeparam name="TInstance">The recursion instance type</typeparam>
        /// <param name="asyncLocal">The async local instance</param>
        /// <param name="instance">The recursion instance</param>
        /// <param name="recursiveFunction">The recursion function</param>
        /// <param name="defaultResult">A default value for when the function is not called</param>
        /// <returns></returns>
        internal static TResult RecursionGuard<TResult, TInstance>(
            this AsyncLocal<HashSet<TInstance>> asyncLocal,
            TInstance instance,
            Func<TInstance, TResult> recursiveFunction,
            TResult defaultResult = default!)
            => asyncLocal.RecursionGuard(instance, recursiveFunction, _ => defaultResult);

        /// <summary>
        /// For situations where a recursive function may be called infinitely, An async local is used to check
        /// for potential infinite-recursion, based on the identity of the instance passed, using a hashset.
        /// </summary>
        /// <typeparam name="TResult">The result-type of the function</typeparam>
        /// <typeparam name="TInstance">The recursion instance type</typeparam>
        /// <param name="asyncLocal">The async local instance</param>
        /// <param name="instance">The recursion instance</param>
        /// <param name="recursiveFunction">The recursion function</param>
        /// <param name="defaultFunction">A default value producer for when the function is not called</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        internal static TResult RecursionGuard<TResult, TInstance>(
            this AsyncLocal<HashSet<TInstance>> asyncLocal,
            TInstance instance,
            Func<TInstance, TResult> recursiveFunction,
            Func<TInstance, TResult> defaultFunction)
        {
            ArgumentNullException.ThrowIfNull(asyncLocal);
            ArgumentNullException.ThrowIfNull(recursiveFunction);
            ArgumentNullException.ThrowIfNull(defaultFunction);
            ArgumentNullException.ThrowIfNull(instance);

            if (asyncLocal.Value is null)
                throw new InvalidOperationException($"Invalid async-local: null");

            if (asyncLocal.Value.Add(instance))
            {
                try
                {
                    return recursiveFunction.Invoke(instance);
                }
                finally
                {
                    asyncLocal.Value.Remove(instance);
                }
            }

            return defaultFunction.Invoke(instance);
        }
    }
}
