using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using System.Collections.Immutable;

namespace Axis.Dia.TypeConverter.Clr
{
    public class SimpleTypeConverter : IClrConverter
    {
        public static SimpleTypeConverter DefaultInstance { get; } = new();

        public bool CanConvert(DiaType sourceType, TypeInfo destinationTypeInfo)
        {
            if (destinationTypeInfo.IsDefault)
                throw new ArgumentException(
                    $"Invalid {nameof(destinationTypeInfo)}: default");

            return sourceType switch
            {
                DiaType.Bool => destinationTypeInfo.Type.IsBoolean(out _),
                DiaType.Blob => destinationTypeInfo.Type.IsBlobType(out _),
                DiaType.Decimal => destinationTypeInfo.Type.IsDecimal(out _),
                DiaType.Duration => destinationTypeInfo.Type.IsTimeSpan(out _),
                DiaType.Int => destinationTypeInfo.Type.IsIntegral(out _),
                DiaType.String => destinationTypeInfo.Type.IsString(),
                DiaType.Symbol => destinationTypeInfo.Type.IsString(),
                DiaType.Timestamp => destinationTypeInfo.Type.IsDateTime(out _),
                _ => false
            };
        }

        public object? ToClr(IDiaValue sourceInstance, TypeInfo destinationTypeInfo, ConverterContext context)
        {
            ArgumentNullException.ThrowIfNull(sourceInstance);

            if (!CanConvert(sourceInstance.Type, destinationTypeInfo))
                throw new InvalidOperationException(
                    $"Invalid Dia - Simple-Type conversion [source: {sourceInstance.Type}, destination: {destinationTypeInfo.Type}]");

            if (sourceInstance.As<INullable>().IsNull)
                return null;

            else if (destinationTypeInfo.Type.IsBoolean(out _))
            {
                return sourceInstance.As<Core.Types.Boolean>().Value;
            }

            else if (destinationTypeInfo.Type.IsString())
            {
                return sourceInstance switch
                {
                    Core.Types.String @string => @string.Value,

                    //Core.Types.Symbol symbol
                    _ => sourceInstance.As<Symbol>().Value,
                };
            }

            else if (destinationTypeInfo.Type.IsIntegral(out var type))
            {
                var bigInt = sourceInstance.As<Core.Types.Integer>().Value!;

                return 
                    typeof(byte).Equals(type) ? (object)(byte)bigInt :
                    typeof(sbyte).Equals(type) ? (object)(sbyte)bigInt :
                    typeof(short).Equals(type) ? (object)(short)bigInt :
                    typeof(ushort).Equals(type) ? (object)(ushort)bigInt :
                    typeof(int).Equals(type) ? (object)(int)bigInt :
                    typeof(uint).Equals(type) ? (object)(uint)bigInt :
                    typeof(long).Equals(type) ? (object)(long)bigInt :
                    typeof(ulong).Equals(type) ? (object)(ulong)bigInt :
                    (object)bigInt;
            }

            else if (destinationTypeInfo.Type.IsDecimal(out type))
            {
                var bigDecimal = sourceInstance.As<Core.Types.Decimal>().Value!;
                return
                    typeof(Half).Equals(type) ? (object)(Half)bigDecimal :
                    typeof(float).Equals(type) ? (object)(float)bigDecimal :
                    typeof(double).Equals(type) ? (object)(double)bigDecimal :
                    typeof(decimal).Equals(type) ? (object)(decimal)bigDecimal :
                    bigDecimal;
            }

            else if (destinationTypeInfo.Type.IsDateTime(out type))
            {
                var timestamp = sourceInstance.As<Timestamp>().Value!;
                return typeof(DateTime).Equals(type)
                    ? (object)timestamp.Value.DateTime
                    : (object)timestamp;
            }

            else if (destinationTypeInfo.Type.IsTimeSpan(out type))
            {
                return sourceInstance.As<Duration>().Value;
            }

            #region byte sequence types
            else if (typeof(byte[]).Equals(destinationTypeInfo.Type))
                return sourceInstance.As<Blob>().Value!.Value.ToArray();

            else if (typeof(ImmutableArray<byte>).Equals(destinationTypeInfo.Type)
                || typeof(ImmutableArray<byte>?).Equals(destinationTypeInfo.Type))
                return sourceInstance.As<Blob>().Value!.Value;

            else if (typeof(ImmutableList<byte>).Equals(destinationTypeInfo.Type))
                return sourceInstance.As<Blob>().Value!.Value.ToImmutableList();

            else //if (typeof(List<byte>).Equals(destinationTypeInfo.Type))
                return sourceInstance.As<Blob>().Value!.Value;
            #endregion
        }
    }
}
