using Axis.Dia.Contracts;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Convert.Type.Clr
{
    /// <summary>
    /// 
    /// </summary>
    public class ReferenceTracker
    {
        private readonly Dictionary<Guid, ITrackingInfo> map = new();

        internal bool IsTracked(IDiaReference linkedReference, out ITrackingInfo? info)
        {
            if (linkedReference is null)
                throw new ArgumentNullException(nameof(linkedReference));

            if (!linkedReference.IsLinked)
                throw new ArgumentException($"Invalid (unlinked) reference supplied");

            return map.TryGetValue(linkedReference.ValueAddress, out info);
        }

        /// <summary>
        /// Registers an initialize if
        /// <list type="number">
        /// <item><paramref name="value"/> is an instance of <see cref="IDiaReference"/></item>
        /// <item>The reference is linked</item>
        /// <item>The reference is tracked</item>
        /// <item>The tracking info is an instance of <see cref="ITrackingInfo.Placeholder"/></item>
        /// </list>
        /// </summary>
        /// <param name="value">The value to try tracking</param>
        /// <param name="initializer">The call back to register if all conditions are met</param>
        /// <returns>True if the call back was registered, false otherwise</returns>
        public bool TryRegisterLazyReferenceInitializer(
            IDiaValue value,
            Action<object?> initializer)
        {
            if (value is IDiaReference linkedRef
                && linkedRef.IsLinked
                && IsTracked(linkedRef, out var info)
                && info is ITrackingInfo.Placeholder placeholder)
            {
                placeholder.RegisterAssignmentCallback(initializer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tracks the given <paramref name="linkedReference"/> instance as a <see cref="ITrackingInfo.Placeholder"/>, then call the 
        /// <paramref name="converter"/> delegate to produce the value, which is in turn returned, after replacing the <see cref="ITrackingInfo.Placeholder"/>
        /// with the produced value.
        /// <para/>
        /// Within the scope of this method, all other "child" conversions that encounter a reference will have access to the created
        /// <see cref="ITrackingInfo.Placeholder"/>, with whom thay may choose to register call-backs to utilize the subsequently produced value.
        /// </summary>
        /// <param name="linkedReference">The reference to track</param>
        /// <param name="converter">The converter delegate</param>
        /// <returns>The converted value</returns>
        /// <exception cref="ArgumentException"></exception>
        internal IResult<object?> TrackConversion(
            IDiaReference linkedReference,
            Func<IResult<object?>> converter)
        {
            if (linkedReference is null)
                throw new ArgumentNullException(nameof(linkedReference));

            if (!linkedReference.IsLinked)
                throw new ArgumentException($"Invalid (unlinked) reference supplied");

            if (converter is null)
                throw new ArgumentNullException(nameof(converter));

            if (map.TryGetValue(linkedReference.ValueAddress, out _))
                return Result.Of<object?>(new InvalidOperationException(
                    $"The given referene is already tracked: {linkedReference.ValueAddress}"));

            map[linkedReference.ValueAddress] = new ITrackingInfo.Placeholder();

            var result = converter
                .Invoke()
                .Map(value => ApplyPlaceholderValue(linkedReference, value));

            // if the result is erroneous, remove the tracking info
            result.ConsumeError(err => map.Remove(linkedReference.ValueAddress));

            return result;
        }

        private object? ApplyPlaceholderValue(IDiaReference linkedReference, object? value)
        {
            if (!map.TryGetValue(linkedReference.ValueAddress, out var trackingInfo))
                throw new InvalidOperationException($"The given reference is not being tracked");

            if (trackingInfo is ITrackingInfo.Placeholder placeholder)
            {
                placeholder.ApplyValue(value);
                map[linkedReference.ValueAddress] = new ITrackingInfo.Value(value);
                return value;
            }
            else throw new InvalidOperationException($"The given reference is not tracked by a placeholder");
        }
    }

    internal interface ITrackingInfo
    {
        internal readonly struct Value : ITrackingInfo
        {
            private readonly object? value;

            internal object? TrackedValue => value;

            internal Value(object? value)
            {
                this.value = value;
            }
        }

        internal class Placeholder: ITrackingInfo
        {
            private readonly List<Action<object?>> assignmentCallbacks = new();
            private bool isApplied = false;

            internal int CallbackCount => assignmentCallbacks.Count;

            internal Placeholder()
            {
            }

            /// <summary>
            /// Register call backs that assign the value to an object when it becomes available.
            /// </summary>
            /// <param name="callback">The call back</param>
            /// <returns>This instance</returns>
            /// <exception cref="InvalidOperationException"></exception>
            internal Placeholder RegisterAssignmentCallback(Action<object?> callback)
            {
                ArgumentNullException.ThrowIfNull(callback);

                if (isApplied)
                    throw new InvalidOperationException($"Invalid placeholder state: cannot register callbacks on applied placeholder");

                assignmentCallbacks.Add(callback);

                return this;
            }

            internal void ApplyValue(object? value)
            {
                if (!isApplied)
                    assignmentCallbacks.ForEach(callback => callback.Invoke(value));

                isApplied = true;
            }
        }
    }
}
