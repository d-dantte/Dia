using Axis.Dia.Core.Utils;
using Axis.Luna.Common;
using System.Numerics;

namespace Axis.Dia.Core.Convert.Axon.Serializers
{
    public class IntegerSerializer : ISerializer<Types.Integer>
    {
        private static readonly string NullTypeText = $"*.{DiaType.Int.ToString().ToLower()}";

        public static string Serialize(Types.Integer value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)}::"
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            if (BigInteger.Zero.Equals(value.Value!.Value))
                return (context.Options.Ints.Style, context.Options.Ints.UseCanonicalForm) switch
                {
                    (Options.IntegerStyle.Decimal, false) => $"{attributeText}0",
                    (Options.IntegerStyle.Decimal, true) => $"'#{DiaType.Int} {attributeText}0'",
                    (Options.IntegerStyle.Hex, false) => $"{attributeText}0x0",
                    (Options.IntegerStyle.Hex, true) => $"'#{DiaType.Int} {attributeText}0x0'",
                    (Options.IntegerStyle.Binary, false) => $"{attributeText}0b0",
                    (Options.IntegerStyle.Binary, true) => $"'#{DiaType.Int} {attributeText}0b0'",

                    _ => throw new InvalidOperationException($"Invalid integer style: {context.Options.Ints.Style}")
                };

            var bigInt = value.Value!.Value;
            var intText = context.Options.Ints.Style switch
            {
                Options.IntegerStyle.Decimal => context.Options.Ints.UseDigitSeparator switch
                {
                    false => bigInt.ToString(),
                    true => BigInteger
                        .Abs(value.Value!.Value)
                        .ToString()
                        .Reverse()
                        .BatchGroup(3)
                        .Select(batchInfo => batchInfo.Batch.JoinUsing())
                        .JoinUsing("_")
                        .Reverse()
                        .ApplyTo(dec => $"{(bigInt.IsNegative() ? "-": "")}{dec}")
                },

                Options.IntegerStyle.Hex => value.Value!.Value
                    .ToString("X")
                    .ApplyTo(hex => context.Options.Ints.UseDigitSeparator switch
                    {
                        false => $"0x{hex}",
                        true => hex
                            .Reverse()
                            .BatchGroup(2)
                            .Select(batchInfo => batchInfo.Batch.JoinUsing())
                            .JoinUsing("_")
                            .Reverse()
                            .ApplyTo(hhex => $"0x{hhex}")
                    }),

                Options.IntegerStyle.Binary => value.Value!.Value
                    .ApplyTo(BitSequence.Of)
                    .SignificantBits
                    .Select(bit => bit switch {true => '1', false => '0'})
                    .BatchGroup(4)
                    .Select(batchInfo => batchInfo.Batch.JoinUsing())
                    .JoinUsing("_")
                    .Reverse()
                    .ApplyTo(hhex => $"0b{hhex}"),

                _ => throw new InvalidOperationException($"Invalid integer style: {context.Options.Ints.Style}")
            };

            intText = context.Options.Ints.UseCanonicalForm
                ? $"'#{DiaType.Int} {intText}'"
                : intText;

            return $"{attributeText}{intText}";
        }
    }
}
