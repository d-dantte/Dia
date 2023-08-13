using Axis.Luna.Common.Numerics;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.Convert.Type
{
    internal static class Extensions
    {

        internal static bool IsIntegral(this System.Type clrType, out System.Type actualType)
        {
            actualType = clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrType.GetGenericArguments()[0]
                : clrType;

            return typeof(byte).Equals(actualType)
                || typeof(sbyte).Equals(actualType)
                || typeof(short).Equals(actualType)
                || typeof(ushort).Equals(actualType)
                || typeof(int).Equals(actualType)
                || typeof(uint).Equals(actualType)
                || typeof(long).Equals(actualType)
                || typeof(ulong).Equals(actualType)
                || typeof(BigInteger).Equals(actualType);
        }

        internal static bool IsDecimal(this System.Type clrType, out System.Type actualType)
        {
            actualType = clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrType.GetGenericArguments()[0]
                : clrType;

            return typeof(Half).Equals(actualType)
                || typeof(float).Equals(actualType)
                || typeof(double).Equals(actualType)
                || typeof(decimal).Equals(actualType)
                || typeof(BigDecimal).Equals(actualType);
        }

        internal static bool IsBoolean(this System.Type clrType, out System.Type actualType)
        {
            actualType = clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrType.GetGenericArguments()[0]
                : clrType;

            return typeof(bool).Equals(actualType);
        }

        internal static bool IsDateTime(this System.Type clrType, out System.Type actualType)
        {
            actualType = clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrType.GetGenericArguments()[0]
                : clrType;

            return typeof(DateTime).Equals(actualType)
                || typeof(DateTimeOffset).Equals(actualType);
        }

        internal static bool IsString(this System.Type clrType) => typeof(string).Equals(clrType);

        internal static bool IsEnumType(this System.Type clrType, out System.Type actualType)
        {
            actualType = clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrType.GetGenericArguments()[0]
                : clrType;

            return actualType.IsEnum;
        }

        /// <summary>
        /// Gets the category of the given type
        /// </summary>
        /// <param name="clrType">the type who's category is sought</param>
        /// <returns>The category</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static TypeCategory GetCategory(this System.Type clrType)
        {
            if (clrType is null)
                throw new ArgumentNullException(nameof(clrType));

            var type = clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrType.GetGenericArguments()[0]
                : clrType;

            if (type.IsPrimitive())
                return TypeCategory.Primitive;

            if (type.IsEnum)
                return TypeCategory.Enum;

            if (type.IsSingleDimensionArray())
                return TypeCategory.SingleDimensionArray;

            if (type.IsMap())
                return type.HastWritableProperties("Item")
                    ? TypeCategory.ComplexMap
                    : TypeCategory.Map;

            if (type.IsCollection())
                return type.HastWritableProperties("Item", "Capacity")
                    ? TypeCategory.ComplexCollection
                    : TypeCategory.Collection;

            if (type.IsRecord())
                return TypeCategory.Record;

            return TypeCategory.InvalidType;
        }

        /// <summary>
        /// Indicates if the given Clr type is a dia primitive
        /// </summary>
        /// <param name="type">The clr type</param>
        private static bool IsPrimitive(this System.Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsPrimitive)
                return true;

            if (typeof(DateTimeOffset).Equals(type))
                return true;

            if (typeof(DateTime).Equals(type))
                return true;

            if (typeof(string).Equals(type))
                return true;

            if (typeof(decimal).Equals(type))
                return true;

            return false;
        }

        /// <summary>
        /// Indicates if the type represents a single dimension array
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <returns>true if it is a single dimension array, false otherwise</returns>
        private static bool IsSingleDimensionArray(this System.Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.IsSZArray;
        }

        /// <summary>
        /// Checks that the type has writable properties, excluding properties found in the <paramref name="excludeProperties"/> set.
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <param name="excludeProperties">properties to exclude</param>
        /// <returns>true if writable properties are present, false otherwise</returns>
        private static bool HastWritableProperties(this System.Type type, params string[] excludeProperties)
        {
            var exclusion = new HashSet<string>(excludeProperties);
            return type
                .GetProperties()
                .Where(prop => !exclusion.Contains(prop.Name))
                .Any(prop => prop.CanWrite);
        }

        /// <summary>
        /// A map is any type that implements <see cref="IDictionary{TKey, TValue}"/>, where the key is a <see cref="string"/>
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <returns>true if it is a map, false otherwise</returns>
        private static bool IsMap(this System.Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsBaseDictionaryInterface())
                return typeof(string).Equals(type.GetGenericArguments()[0]);

            else return type.ImplementsGenericInterface(typeof(IDictionary<,>))
                && typeof(string).Equals(type
                    .GetGenericInterface(typeof(IDictionary<,>))
                    .GetGenericArguments()[0]);
        }

        /// <summary>
        /// A collection is any non-array type that is, or implements <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <returns>true if it is a list, false otherwise</returns>
        private static bool IsCollection(this System.Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return
                !type.IsArray
                && (type.ImplementsGenericInterface(typeof(IEnumerable<>))
                || type.HasGenericInterfaceDefinition(typeof(IEnumerable<>)));
        }

        /// <summary>
        /// A POCO that isn't a delegate or an array - also doesn't implement <see cref="IEnumerable{T}"/>, or <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static bool IsRecord(this System.Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type.Extends(typeof(Delegate)))
                return false;

            if (type.IsArray)
                return false;

            return true;
        }



        private static bool IsBaseDictionaryInterface(this System.Type type)
        {
            return type.IsGenericType
                && typeof(IDictionary<,>).Equals(type.GetGenericTypeDefinition());
        }
    }
}
