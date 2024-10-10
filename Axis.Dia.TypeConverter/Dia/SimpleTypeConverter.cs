using Axis.Dia.Core.Contracts;
using Axis.Luna.Numerics;
using System.Collections.Immutable;
using System.Numerics;

namespace Axis.Dia.TypeConverter.Dia
{
    public class SimpleTypeConverter :
        IDiaConverter,
        IDefaultInstance<SimpleTypeConverter>
    {
        public static SimpleTypeConverter DefaultInstance { get; } = new();

        public bool CanConvert(TypeInfo sourceTypeInfo)
        {
            if (sourceTypeInfo.IsDefault)
                throw new ArgumentException(
                    $"Invalid {nameof(sourceTypeInfo)}: default");

            return sourceTypeInfo.Type.IsSimpleType();
        }

        public IDiaValue ToDia(TypeInfo sourceTypeInfo, object? sourceInstance, ConverterContext context)
        {
            if (!CanConvert(sourceTypeInfo))
                throw new InvalidOperationException(
                    $"Invalid source-type: '{sourceTypeInfo.Type}' is not a simple type");

            if (sourceTypeInfo.Type.IsBoolean(out _))
                return sourceInstance switch
                {
                    null => Core.Types.Boolean.Null(),
                    bool @bool => Core.Types.Boolean.Of(@bool),
                    _ => throw new InvalidOperationException(
                        $"Type mismatch [expected: {sourceTypeInfo.Type}, actual: {sourceInstance!.GetType()}]")
                };

            if (sourceTypeInfo.Type.IsString())
                return sourceInstance switch
                {
                    null => Core.Types.String.Null(),
                    string @string => Core.Types.String.Of(@string),
                    _ => throw new InvalidOperationException(
                        $"Type mismatch [expected: {sourceTypeInfo.Type}, actual: {sourceInstance!.GetType()}]")
                };

            if (sourceTypeInfo.Type.IsIntegral(out _))
                return sourceInstance switch
                {
                    null => Core.Types.Integer.Null(),
                    byte @byte => Core.Types.Integer.Of(@byte),
                    sbyte @sbyte => Core.Types.Integer.Of(@sbyte),
                    short @short => Core.Types.Integer.Of(@short),
                    ushort @ushort => Core.Types.Integer.Of(@ushort),
                    int @int => Core.Types.Integer.Of(@int),
                    uint @uint => Core.Types.Integer.Of(@uint),
                    long @long => Core.Types.Integer.Of(@long),
                    ulong @ulong => Core.Types.Integer.Of(@ulong),
                    BigInteger bi => Core.Types.Integer.Of(bi),
                    _ => throw new InvalidOperationException(
                        $"Type mismatch [expected: {sourceTypeInfo.Type}, actual: {sourceInstance!.GetType()}]")
                };

            if (sourceTypeInfo.Type.IsDecimal(out _))
                return sourceInstance switch
                {
                    null => Core.Types.Decimal.Null(),
                    Half half => Core.Types.Decimal.Of(half),
                    float @float => Core.Types.Decimal.Of(@float),
                    double @double => Core.Types.Decimal.Of(@double),
                    decimal @decimal => Core.Types.Decimal.Of(@decimal),
                    BigDecimal bd => Core.Types.Decimal.Of(bd),
                    _ => throw new InvalidOperationException(
                        $"Type mismatch [expected: {sourceTypeInfo.Type}, actual: {sourceInstance!.GetType()}]")
                };

            if (sourceTypeInfo.Type.IsDateTime(out _))
                return sourceInstance switch
                {
                    null => Core.Types.Timestamp.Null(),
                    DateTime datetime => Core.Types.Timestamp.Of(datetime),
                    DateTimeOffset datetime => Core.Types.Timestamp.Of(datetime),
                    _ => throw new InvalidOperationException(
                        $"Type mismatch [expected: {sourceTypeInfo.Type}, actual: {sourceInstance!.GetType()}]")
                };

            if (sourceTypeInfo.Type.IsTimeSpan(out _))
                return sourceInstance switch
                {
                    null => Core.Types.Duration.Null(),
                    TimeSpan datetime => Core.Types.Duration.Of(datetime),
                    _ => throw new InvalidOperationException(
                        $"Type mismatch [expected: {sourceTypeInfo.Type}, actual: {sourceInstance!.GetType()}]")
                };

            if (sourceTypeInfo.Type.IsBlobType(out _))
                return sourceInstance switch
                {
                    null => Core.Types.Blob.Null(),
                    byte[] value => Core.Types.Blob.Of(value),
                    ImmutableArray<byte> value => Core.Types.Blob.Of(value),
                    ImmutableList<byte> value => Core.Types.Blob.Of(value),
                    List<byte> value => Core.Types.Blob.Of(value),
                    IEnumerable<byte> value => Core.Types.Blob.Of(value),
                    _ => throw new InvalidOperationException(
                        $"Type mismatch [expected: {sourceTypeInfo.Type}, actual: {sourceInstance!.GetType()}]")
                };

            // This shouldn't be reached
            throw new InvalidOperationException(
                $"Invalid source-type: '{sourceTypeInfo.Type}' is not a simple type");

        }
    }
}
