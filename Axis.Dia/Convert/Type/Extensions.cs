using Axis.Dia.Contracts;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Extensions;
using System.Collections.Immutable;
using System.Numerics;

namespace Axis.Dia.Convert.Type
{
    internal static class Extensions
    {
        #region CLR type tests
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrValueType"></param>
        /// <param name="actualType"></param>
        /// <returns></returns>
        internal static bool IsIntegral(this System.Type clrValueType, out System.Type actualType)
        {
            actualType = clrValueType.IsGenericType && clrValueType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrValueType.GetGenericArguments()[0]
                : clrValueType;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrValueType"></param>
        /// <param name="actualType"></param>
        /// <returns></returns>
        internal static bool IsDecimal(this System.Type clrValueType, out System.Type actualType)
        {
            actualType = clrValueType.IsGenericType && clrValueType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrValueType.GetGenericArguments()[0]
                : clrValueType;

            return typeof(Half).Equals(actualType)
                || typeof(float).Equals(actualType)
                || typeof(double).Equals(actualType)
                || typeof(decimal).Equals(actualType)
                || typeof(BigDecimal).Equals(actualType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrValueType"></param>
        /// <param name="actualType"></param>
        /// <returns></returns>
        internal static bool IsBoolean(this System.Type clrValueType, out System.Type actualType)
        {
            actualType = clrValueType.IsGenericType && clrValueType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrValueType.GetGenericArguments()[0]
                : clrValueType;

            return typeof(bool).Equals(actualType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrValueType"></param>
        /// <param name="actualType"></param>
        /// <returns></returns>
        internal static bool IsDateTime(this System.Type clrValueType, out System.Type actualType)
        {
            actualType = clrValueType.IsGenericType && clrValueType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrValueType.GetGenericArguments()[0]
                : clrValueType;

            return typeof(DateTime).Equals(actualType)
                || typeof(DateTimeOffset).Equals(actualType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrValueType"></param>
        /// <param name="actualType"></param>
        /// <returns></returns>
        internal static bool IsEnumType(this System.Type clrValueType, out System.Type actualType)
        {
            actualType = clrValueType.IsGenericType && clrValueType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrValueType.GetGenericArguments()[0]
                : clrValueType;

            return actualType.IsEnum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrRefType"></param>
        /// <returns></returns>
        internal static bool IsString(this System.Type clrRefType) => typeof(string).Equals(clrRefType);

        internal static bool IsByteSequenceType(this System.Type clrType, out System.Type actualType)
        {
            actualType = clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrType.GetGenericArguments()[0]
                : clrType;

            if (actualType == null)
                throw new ArgumentNullException(nameof(actualType));

            if (typeof(byte[]).Equals(actualType))
                return true;

            else if (typeof(ImmutableArray<byte>).Equals(actualType))
                return true;

            else if (typeof(ImmutableList<byte>).Equals(actualType))
                return true;

            else if (typeof(List<byte>).Equals(actualType))
                return true;

            return false;
        }

        /// <summary>
        /// Gets the category of the given type
        /// </summary>
        /// <param name="clrType">the type who's category is sought</param>
        /// <returns>The category</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TypeCategory GetTypeCategory(this System.Type clrType)
        {
            if (clrType is null)
                throw new ArgumentNullException(nameof(clrType));

            var actualType = clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrType.GetGenericArguments()[0]
                : clrType;

            if (actualType.IsPrimitive())
                return TypeCategory.Primitive;

            if (actualType.IsEnum)
                return TypeCategory.Enum;

            if (actualType.IsSingleDimensionArray())
                return TypeCategory.SingleDimensionArray;

            if (actualType.IsMap())
                return TypeCategory.Map;

            if (actualType.IsCollection())
                return TypeCategory.Collection;

            if (actualType.IsRecord())
                return TypeCategory.Record;

            return TypeCategory.InvalidType;
        }
        #endregion

        #region DIA Type Tests

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsSimpleDiaType(this DiaType type)
        {
            return type switch
            {
                DiaType.Int
                or DiaType.Decimal
                or DiaType.Instant
                or DiaType.Bool
                or DiaType.String
                or DiaType.Symbol
                or DiaType.Clob
                or DiaType.Blob => true,
                _ => false
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsComplexDiaType(this DiaType type)
        {
            return DiaType.Record.Equals(type) || DiaType.List.Equals(type);
        }
        #endregion

        #region Misc

        /// <summary>
        /// Verifies that the given GENERIC type has the supplied generic type definition defined on it.
        /// PS: this method will be featured in a future version of <c>Axis.Luna.Extension</c>.
        /// </summary>
        /// <param name="genericType">The generic interface</param>
        /// <param name="genericTypeDefinition">The generic interface type definition</param>
        internal static bool HasGenericTypeDefinition(this System.Type genericType, System.Type genericTypeDefinition)
        {
            if (genericType == null)
                throw new ArgumentNullException(nameof(genericType));

            if (genericTypeDefinition == null)
                throw new ArgumentNullException(nameof(genericTypeDefinition));

            if (genericType.IsInterface || !genericType.IsGenericType)
                return false;

            if (!genericTypeDefinition.IsGenericTypeDefinition)
                return false;

            return genericType.GetGenericTypeDefinition().Equals(genericTypeDefinition);
        }
        #endregion

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
