using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Common;

namespace Axis.Dia.Convert.Type.Dia
{
    /// <summary>
    /// Type that facilitates tracking instances of Clr objects, assigning un-linked <see cref="ReferenceValue"/> instances to them.
    /// </summary>
    public class ObjectTracker
    {
        private readonly Dictionary<ObjectKey, IDiaReference> map = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal bool IsTracked(object obj, out IDiaReference? reference)
        {
            return map.TryGetValue(ObjectKey.Of(obj), out reference);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal bool TryTrack(object obj, out IDiaReference? reference)
        {
            if (IsTracked(obj, out reference))
                return false;

            reference = map[ObjectKey.Of(obj)] = ReferenceValue.Of(Guid.NewGuid());
            return true;
        }


        #region Nested types
        internal readonly struct ObjectKey : IDefaultValueProvider<ObjectKey>
        {
            internal int HashCode { get; }

            internal System.Type Type { get; }

            #region DefaultValueProvider
            public bool IsDefault => default(ObjectKey).Equals(this);

            public static ObjectKey Default => default;
            #endregion

            private ObjectKey(object @object)
            {
                ArgumentNullException.ThrowIfNull(@object);

                HashCode = @object.GetHashCode();
                Type = @object.GetType();
            }

            internal static ObjectKey Of(object @object) => new(@object);
        }
        #endregion
    }
}
