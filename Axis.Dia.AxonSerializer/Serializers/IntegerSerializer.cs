using Axis.Dia.Core;
using Axis.Luna.BitSequence;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.Axon.Serializers
{
    public class IntegerSerializer : IValueSerializer<Core.Types.Integer>
    {
        private static readonly string NullTypeText = $"null.{DiaType.Int.ToString().ToLower()}";

        public static string Serialize(Core.Types.Integer value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)} "
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            if (BigInteger.Zero.Equals(value.Value!.Value))
                return context.Options.Ints.Style switch
                {
                    Options.IntegerStyle.Decimal => $"{attributeText}0",
                    Options.IntegerStyle.Hex => $"{attributeText}0x0",
                    Options.IntegerStyle.Binary => $"{attributeText}0b0",

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
                    .TrimStart('0')
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

            return $"{attributeText}{intText}";
        }
    }
}
