using Axis.Dia.Core;
using Axis.Luna.Extensions;
using Axis.Luna.Numerics;
using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;

namespace Axis.Dia.TypeConverter
{
    internal static class Extensions
    {
        #region CLR type tests
        internal static bool IsIntegral(this Type clrValueType, out Type actualType)
        {
            ArgumentNullException.ThrowIfNull(clrValueType);

            actualType = clrValueType.IsGenericType && clrValueType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
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

        internal static bool IsDecimal(this Type clrValueType, out Type actualType)
        {
            ArgumentNullException.ThrowIfNull(clrValueType);

            actualType = clrValueType.IsGenericType && clrValueType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
                ? clrValueType.GetGenericArguments()[0]
                : clrValueType;

            return typeof(Half).Equals(actualType)
                || typeof(float).Equals(actualType)
                || typeof(double).Equals(actualType)
                || typeof(decimal).Equals(actualType)
                || typeof(BigDecimal).Equals(actualType);
        }

        internal static bool IsBoolean(this Type clrValueType, out Type actualType)
        {
            ArgumentNullException.ThrowIfNull(clrValueType);

            actualType = clrValueType.IsGenericType && clrValueType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
                ? clrValueType.GetGenericArguments()[0]
                : clrValueType;

            return typeof(bool).Equals(actualType);
        }

        internal static bool IsDateTime(this Type clrValueType, out Type actualType)
        {
            ArgumentNullException.ThrowIfNull(clrValueType);

            actualType = clrValueType.IsGenericType && clrValueType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
                ? clrValueType.GetGenericArguments()[0]
                : clrValueType;

            return typeof(DateTime).Equals(actualType)
                || typeof(DateTimeOffset).Equals(actualType);
        }

        internal static bool IsTimeSpan(this Type clrValueType, out Type actualType)
        {
            ArgumentNullException.ThrowIfNull(clrValueType);

            actualType = clrValueType.IsGenericType && clrValueType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
                ? clrValueType.GetGenericArguments()[0]
                : clrValueType;

            return typeof(TimeSpan).Equals(actualType);
        }

        internal static bool IsEnumType(this Type clrValueType, out Type actualType)
        {
            ArgumentNullException.ThrowIfNull(clrValueType);

            actualType = clrValueType.IsGenericType && clrValueType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
                ? clrValueType.GetGenericArguments()[0]
                : clrValueType;

            return actualType.IsEnum;
        }

        internal static bool IsString(this Type clrRefType) => typeof(string).Equals(clrRefType);

        internal static bool IsBlobType(this Type clrType, out Type actualType)
        {
            ArgumentNullException.ThrowIfNull(clrType);

            actualType = clrType.IsGenericType && clrType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
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

            else if (typeof(IEnumerable<byte>).Equals(actualType))
                return true;

            return false;
        }

        /// <summary>
        /// Indicates if the type represents a single dimension array
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <returns>true if it is a single dimension array, false otherwise</returns>
        internal static bool IsSingleDimensionArray(this Type type, out Type? itemType)
        {
            itemType = type.IsSZArray
                ? type.GetElementType()
                : null;
            return type.IsSZArray;
        }

        /// <summary>
        /// A map is any type that implements <see cref="IDictionary{TKey, TValue}"/>, where the key is a <see cref="string"/>
        /// </summary>
        internal static bool IsMap(this Type type, out Type? valueType)
        {
            ArgumentNullException.ThrowIfNull(type);

            valueType = null;

            if (type.IsBaseDictionaryInterface())
            {
                valueType = type.GetGenericArguments()[1];
                return typeof(string).Equals(type.GetGenericArguments()[0]);
            }
            else if (type.ImplementsGenericInterface(typeof(IDictionary<,>)))
            {
                var genericDictionary = type.GetGenericInterface(typeof(IDictionary<,>));
                var keyType = genericDictionary.GetGenericArguments()[0];
                valueType = genericDictionary.GetGenericArguments()[1];

                return typeof(string).Equals(keyType);
            }

            return false;
        }

        /// <summary>
        /// A sequence is any non-array type that is, or implements <see cref="IEnumerable{T}"/>.
        /// </summary>
        internal static bool IsSequence(this Type type, out Type? itemType)
        {
            ArgumentNullException.ThrowIfNull(type);

            var enmType = typeof(IEnumerable<>);
            var isSequence = type.HasGenericInterfaceDefined(enmType, out var args) && !type.IsArray;
            itemType = isSequence ? args[0] : null;
            return isSequence;
        }

        /// <summary>
        /// A POCO that isn't a delegate or an array - also doesn't implement <see cref="IEnumerable{T}"/>, or <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        internal static bool IsRecord(this Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            if (type.IsPrimitive)
                return false;

            if (type.IsMap(out _))
                return false;

            if (type.IsSequence(out _))
                return false;

            return type.IsRecord_();
        }

        internal static TypeCategory ToTypeCategory(this Type clrType)
        {
            return ToTypeCategory(clrType, out _);
        }

        internal static TypeCategory ToTypeCategory(this Type clrType, out Type? itemType)
        {
            ArgumentNullException.ThrowIfNull(clrType);

            itemType = null;
            var actualType = clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? clrType.GetGenericArguments()[0]
                : clrType;

            if (actualType.IsSimpleType())
                return TypeCategory.Simple;

            if (actualType.IsEnum)
                return TypeCategory.Enum;

            if (actualType.IsSingleDimensionArray(out itemType))
                return TypeCategory.SingleDimensionArray;

            if (actualType.IsMap(out itemType))
                return TypeCategory.Map;

            if (actualType.IsSequence(out itemType))
                return TypeCategory.Sequence;

            if (actualType.IsRecord_())
                return TypeCategory.Record;

            return TypeCategory.Unknown;
        }

        private static bool IsRecord_(this Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            if (type.Extends(typeof(Delegate)))
                return false;

            if (type.IsArray)
                return false;

            return true;
        }

        internal static TypeInfo ToTypeInfo(this Type clrType) => TypeInfo.Of(clrType);
        #endregion

        #region DIA Type Tests

        /// <summary>
        /// All types except <see cref="DiaType.Record"/> and <see cref="DiaType.Sequence"/> are simple types
        /// </summary>
        internal static bool IsSimpleDiaType(this DiaType type)
        {
            return type switch
            {
                DiaType.Int
                or DiaType.Decimal
                or DiaType.Timestamp
                or DiaType.Bool
                or DiaType.String
                or DiaType.Symbol
                or DiaType.Blob => true,
                _ => false
            };
        }

        internal static bool IsComplexDiaType(this DiaType type)
        {
            return type switch
            {
                DiaType.Record
                or DiaType.Sequence => true,
                _ => false
            };
        }
        #endregion

        #region Misc

        /// <summary>
        /// Verifies that the given GENERIC type has the supplied generic type definition defined on it.
        /// PS: this method will be featured in a future version of <c>Axis.Luna.Extension</c>.
        /// </summary>
        /// <param name="genericType">The generic interface</param>
        /// <param name="genericTypeDefinition">The generic interface type definition</param>
        internal static bool HasGenericTypeDefinition(this Type genericType, Type genericTypeDefinition)
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
        /// Indicates if the given Clr type is equivalent to a dia primitive/simple-type. All types, except for <c>Record</c> and <c>Sequence</c>, are primitives.
        /// </summary>
        /// <param name="type">The clr type</param>
        internal static bool IsSimpleType(this Type type)
        {
            ArgumentNullException.ThrowIfNull(nameof(type));

            return type.IsIntegral(out _)
                || type.IsDecimal(out _)
                || type.IsBoolean(out _)
                || type.IsDateTime(out _)
                || type.IsTimeSpan(out _)
                || type.IsString()
                || type.IsBlobType(out _);
        }

        /// <summary>
        /// Checks that the type has writable properties, excluding properties found in the <paramref name="excludeProperties"/> set.
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <param name="excludeProperties">properties to exclude</param>
        /// <returns>true if writable properties are present, false otherwise</returns>
        internal static bool TryGetWritableProperties(this
            Type type,
            string[] excludeProperties,
            out ImmutableArray<string> writableProperties)
        {
            var exclusion = new HashSet<string>(excludeProperties);
            writableProperties = type
                .GetProperties()
                .Where(prop => !exclusion.Contains(prop.Name))
                .Where(prop => prop.CanWrite)
                .Select(prop => prop.Name)
                .ToImmutableArray();

            return !writableProperties.IsEmpty;
        }

        internal static bool IsBaseDictionaryInterface(this Type type)
        {
            return type.IsGenericType
                && typeof(IDictionary<,>).Equals(type.GetGenericTypeDefinition());
        }

        internal static bool HasGenericInterfaceDefined(
            this Type type,
            Type genericInterfaceDefinition,
            out ImmutableArray<Type> genericTypeArguments)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(genericInterfaceDefinition);

            if (type.IsGenericTypeDefinition)
                throw new ArgumentException(
                    $"Invalid type: {nameof(type)} is a generic type definition");

            if (!genericInterfaceDefinition.IsGenericTypeDefinition)
                throw new ArgumentException(
                    $"Invalid type: {nameof(genericInterfaceDefinition)} is not a generic type definition");

            var interfaces = type.GetInterfaces().AsEnumerable();

            if (type.IsInterface)
                interfaces = interfaces.Prepend(type);

            genericTypeArguments = interfaces
                .Where(i => i.IsGenericType)
                .FirstOrDefault(t => genericInterfaceDefinition.Equals(t.GetGenericTypeDefinition()))
                ?.GetGenericArguments()
                .ToImmutableArray()
                ?? ImmutableArray.Create<Type>();

            return !genericTypeArguments.IsEmpty;
        }

        internal static bool HasNoArgConstructor(this Type type)
        {
            return type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Any(ctor => ctor.GetParameters().IsEmpty());
        }

        internal static void ForEvery(
            this System.Collections.IEnumerable enm,
            Action<object> action)
        {
            ArgumentNullException.ThrowIfNull(enm);
            ArgumentNullException.ThrowIfNull(action);

            foreach(var obj in enm)
            {
                action.Invoke(obj);
            }
        }

        internal static IEnumerable<TItem> SelectObject<TItem>(
            this System.Collections.IEnumerable enm,
            Func<object, TItem> mapper)
        {
            ArgumentNullException.ThrowIfNull(enm);
            ArgumentNullException.ThrowIfNull(mapper);

            foreach (var obj in enm)
                yield return mapper.Invoke(obj);
        }
    }
}
