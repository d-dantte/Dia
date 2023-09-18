using Axis.Dia.Contracts;
using Axis.Dia.Convert.Type.Exceptions;
using Axis.Dia.Types;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Collections.Immutable;
using System.Numerics;

namespace Axis.Dia.Convert.Type.Converters
{
    /// <summary>
    /// Converts dia simple-types into the clr types, and vice-versa.
    /// <para/>
    /// This converter needs not participate in reference tracking during Clr conversion, because any reference to the
    /// simple type will be dereferenced and converted, bearing in mind that since all simple types are self-contained, there is
    /// no chance of a cyclic reference error occuring.
    /// </summary>
    internal class SimpleTypeConverter : IClrConverter, IDiaConverter
    {
        #region IClrConverter
        public bool CanConvert(DiaType sourceType, System.Type destinationType)
        {
            if (destinationType is null)
                throw new ArgumentNullException(nameof(destinationType));

            return sourceType switch
            {
                DiaType.Int => destinationType.IsIntegral(out _),
                DiaType.Decimal => destinationType.IsDecimal(out _),
                DiaType.Instant => destinationType.IsDateTime(out _),
                DiaType.Bool => destinationType.IsBoolean(out _),
                DiaType.String => destinationType.IsString(),
                DiaType.Clob => destinationType.IsString(),
                DiaType.Symbol => destinationType.IsEnumType(out _) || destinationType.IsString(),
                DiaType.Blob => destinationType.IsByteSequenceType(out _),
                _ => false
            };
        }

        /// <summary>
        /// Converts the given <paramref name="sourceInstance"/> instance to a clr value.
        /// </summary>
        /// <param name="destinationType">The type to convert the given ion instance into</param>
        /// <param name="sourceInstance">The ion instance</param>
        /// <param name="context">The conversion context</param>
        /// <returns>The converted clr value</returns>
        /// <exception cref="ArgumentNullException">
        ///     If either <paramref name="destinationType"/> or <paramref name="sourceInstance"/> are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If the conversion fails
        /// </exception>
        public IResult<object?> ToClr(
            IDiaValue sourceInstance,
            System.Type destinationType,
            Clr.ConverterContext context)
        {
            if (sourceInstance is null)
                throw new ArgumentNullException(nameof(sourceInstance));

            if (destinationType is null)
                throw new ArgumentNullException(nameof(destinationType));

            if (!sourceInstance.Type.IsSimpleDiaType())
                throw new ArgumentException($"Invalid source type: {sourceInstance.Type}");

            if (!CanConvert(sourceInstance.Type, destinationType))
                return Result.Of<object?>(new IncompatibleClrConversionException(
                    sourceInstance.Type,
                    destinationType));

            if (sourceInstance.IsNull)
                return Result.Of((object?)null);

            if (destinationType.IsBoolean(out _))
            {
                return sourceInstance
                    .As<BoolValue>()
                    .ApplyTo(v =>Result.Of<object?>(v.Value));
            }

            if (destinationType.IsString())
            {
                return sourceInstance switch
                {
                    StringValue @string => Result.Of<object?>(@string.Value),
                    SymbolValue symbol => Result.Of<object?>(symbol.Value),
                    ClobValue clob => Result.Of<object?>(clob.Value),
                    _ => Result.Of<object?>(new IncompatibleClrConversionException(
                        sourceInstance.Type,
                        destinationType))
                };
            }

            if (destinationType.IsIntegral(out var type))
            {
                if (!DiaType.Int.Equals(sourceInstance.Type))
                    return Result.Of<object?>(new IncompatibleClrConversionException(
                        sourceInstance.Type,
                        destinationType));

                var bigInt = sourceInstance.As<IntValue>().Value!;

                return Result.Of<object?>(
                    typeof(byte).Equals(type) ? (object)(byte)bigInt :
                    typeof(sbyte).Equals(type) ? (object)(sbyte)bigInt :
                    typeof(short).Equals(type) ? (object)(short)bigInt :
                    typeof(ushort).Equals(type) ? (object)(ushort)bigInt :
                    typeof(int).Equals(type) ? (object)(int)bigInt :
                    typeof(uint).Equals(type) ? (object)(uint)bigInt :
                    typeof(long).Equals(type) ? (object)(long)bigInt :
                    typeof(ulong).Equals(type) ? (object)(ulong)bigInt:
                    (object)bigInt);
            }

            if (destinationType.IsDecimal(out type))
            {
                if (!DiaType.Decimal.Equals(sourceInstance.Type))
                    return Result.Of<object?>(new IncompatibleClrConversionException(
                        sourceInstance.Type,
                        destinationType));

                var bigDecimal = sourceInstance.As<DecimalValue>().Value!;
                return Result.Of<object?>(
                    typeof(Half).Equals(type) ? (object)(Half)bigDecimal :
                    typeof(float).Equals(type) ? (object)(float)bigDecimal :
                    typeof(double).Equals(type) ? (object)(double)bigDecimal :
                    typeof(decimal).Equals(type) ? (object)(decimal)bigDecimal :
                    bigDecimal);
            }

            if (destinationType.IsDateTime(out type))
            {
                if (!DiaType.Instant.Equals(sourceInstance.Type))
                    return Result.Of<object?>(new IncompatibleClrConversionException(
                        sourceInstance.Type,
                        destinationType));

                var timestamp = sourceInstance.As<InstantValue>().Value!;
                return Result.Of<object?>(typeof(DateTime).Equals(type) 
                    ? (object)timestamp.Value.DateTime
                    : (object)timestamp);
            }

            #region byte sequence types
            if (typeof(byte[]).Equals(destinationType))
            {
                return sourceInstance switch
                {
                    BlobValue blob => Result.Of<object?>(blob.Value!.Clone()),
                    _ => Result.Of<object?>(new IncompatibleClrConversionException(
                        sourceInstance.Type,
                        destinationType))
                };
            }

            if (typeof(ImmutableArray<byte>).Equals(destinationType)
                || typeof(ImmutableArray<byte>?).Equals(destinationType))
            {
                return sourceInstance switch
                {
                    BlobValue blob => Result.Of<object?>(ImmutableArray.Create(blob.Value!)),
                    _ => Result.Of<object?>(new IncompatibleClrConversionException(
                        sourceInstance.Type,
                        destinationType))
                };
            }

            if (typeof(ImmutableList<byte>).Equals(destinationType))
            {
                return sourceInstance switch
                {
                    BlobValue blob => Result.Of<object?>(ImmutableList.Create(blob.Value!)),
                    _ => Result.Of<object?>(new IncompatibleClrConversionException(
                        sourceInstance.Type,
                        destinationType))
                };
            }

            if (typeof(List<byte>).Equals(destinationType))
            {
                return sourceInstance switch
                {
                    BlobValue blob => Result.Of<object?>(new List<byte>(blob.Value!)),
                    _ => Result.Of<object?>(new IncompatibleClrConversionException(
                        sourceInstance.Type,
                        destinationType))
                };
            }
            #endregion

            return Result.Of<object?>(
                new IncompatibleClrConversionException(
                    sourceInstance.Type,
                    destinationType));
        }
        #endregion

        #region IDiaConverter
        public bool CanConvert(System.Type sourceType)
        {
            if (sourceType is null)
                throw new ArgumentNullException(nameof(sourceType));

            return sourceType.IsBoolean(out _)
                || sourceType.IsDecimal(out _)
                || sourceType.IsDateTime(out _)
                || sourceType.IsIntegral(out _)
                || sourceType.IsEnumType(out _)
                || sourceType.IsByteSequenceType(out _)
                || sourceType.IsString();
        }

        /// <summary>
        /// Converts the given clr <paramref name="sourceInstance"/> into a <see cref="IDiaValue"/> instance.
        /// </summary>
        /// <param name="sourceType">The type from which the conversion is made</param>
        /// <param name="sourceInstance">The instance to be converted</param>
        /// <param name="context">The conversion context</param>
        /// <returns>The converted ion value</returns>
        /// <exception cref="ArgumentNullException">
        ///     If either <paramref name="destinationType"/> or <paramref name="ion"/> are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If the conversion fails
        /// </exception>
        public IResult<IDiaValue> ToDia(System.Type sourceType, object? sourceInstance, Dia.ConverterContext context)
        {
            if (sourceType is null)
                throw new ArgumentNullException(nameof(sourceType));

            if (!CanConvert(sourceType))
                return Result.Of<IDiaValue>(new UnknownClrSourceTypeException(sourceType));

            if (sourceType.IsBoolean(out _))
                return sourceInstance switch
                {
                    null => Result.Of<IDiaValue>(BoolValue.Null()),
                    bool @bool => Result.Of<IDiaValue>(BoolValue.Of(@bool)),
                    _ => Result.Of<IDiaValue>(new TypeMismatchException(sourceType, sourceInstance.GetType()))
                };

            if (sourceType.IsString())
                return sourceInstance switch
                {
                    null => Result.Of<IDiaValue>(StringValue.Null()),
                    string @string => Result.Of<IDiaValue>(StringValue.Of(@string)),
                    _ => Result.Of<IDiaValue>(new TypeMismatchException(sourceType, sourceInstance.GetType()))
                };

            if (sourceType.IsIntegral(out _))
                return sourceInstance switch
                {
                    null => Result.Of<IDiaValue>(IntValue.Null()),
                    byte @byte => Result.Of<IDiaValue>(IntValue.Of(@byte)),
                    sbyte @sbyte => Result.Of<IDiaValue>(IntValue.Of(@sbyte)),
                    short @short => Result.Of<IDiaValue>(IntValue.Of(@short)),
                    ushort @ushort => Result.Of<IDiaValue>(IntValue.Of(@ushort)),
                    int @int => Result.Of<IDiaValue>(IntValue.Of(@int)),
                    uint @uint => Result.Of<IDiaValue>(IntValue.Of(@uint)),
                    long @long => Result.Of<IDiaValue>(IntValue.Of(@long)),
                    ulong @ulong => Result.Of<IDiaValue>(IntValue.Of(@ulong)),
                    BigInteger bi => Result.Of<IDiaValue>(IntValue.Of(bi)),
                    _ => Result.Of<IDiaValue>(new TypeMismatchException(sourceType, sourceInstance.GetType()))
                };

            if (sourceType.IsDecimal(out _))
                return sourceInstance switch
                {
                    null => Result.Of<IDiaValue>(DecimalValue.Null()),
                    Half half => Result.Of<IDiaValue>(DecimalValue.Of(half)),
                    float @float => Result.Of<IDiaValue>(DecimalValue.Of(@float)),
                    double @double => Result.Of<IDiaValue>(DecimalValue.Of(@double)),
                    decimal @decimal => Result.Of<IDiaValue>(DecimalValue.Of(@decimal)),
                    BigDecimal bd => Result.Of<IDiaValue>(DecimalValue.Of(bd)),
                    _ => Result.Of<IDiaValue>(new TypeMismatchException(sourceType, sourceInstance.GetType()))
                };

            if (sourceType.IsDateTime(out _))
                return sourceInstance switch
                {
                    null => Result.Of<IDiaValue>(InstantValue.Null()),
                    DateTime datetime => Result.Of<IDiaValue>(InstantValue.Of(datetime)),
                    DateTimeOffset datetime => Result.Of<IDiaValue>(InstantValue.Of(datetime)),
                    _ => Result.Of<IDiaValue>(new TypeMismatchException(sourceType, sourceInstance.GetType()))
                };

            if (sourceType.IsByteSequenceType(out _))
                return sourceInstance switch
                {
                    null => Result.Of<IDiaValue>(BlobValue.Null()),
                    byte[] value => Result.Of<IDiaValue>(BlobValue.Of(value)),
                    ImmutableArray<byte> value => Result.Of<IDiaValue>(BlobValue.Of(value.ToArray())),
                    ImmutableList<byte> value => Result.Of<IDiaValue>(BlobValue.Of(value.ToArray())),
                    List<byte> value => Result.Of<IDiaValue>(BlobValue.Of(value.ToArray())),
                    _ => Result.Of<IDiaValue>(new TypeMismatchException(sourceType, sourceInstance.GetType()))
                };

            return Result.Of<IDiaValue>(new UnknownClrSourceTypeException(sourceType));
        }
        #endregion
    }
}
