using Axis.Dia.Types;

namespace Axis.Dia.Contracts
{
    /// <summary>
    /// The base contract for Dia values
    /// </summary>
    public interface IDiaValue
    {
        #region NullOf
        /// <summary>
        /// Creates null values for the given <see cref="DiaType"/>, and attributes
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="annotations">The list of annotations</param>
        /// <returns>The null value</returns>
        /// <exception cref="ArgumentException">If the type is invalid</exception>
        public static IDiaValue NullOf(DiaType type, params Annotation[] attributes)
        {
            return type switch
            {
                DiaType.Bool => BoolValue.Null(attributes),
                DiaType.Int => IntValue.Null(attributes),
                DiaType.Decimal => DecimalValue.Null(attributes),
                DiaType.Instant => InstantValue.Null(attributes),
                DiaType.String => StringValue.Null(attributes),
                DiaType.Symbol => SymbolValue.Null(),
                DiaType.Clob => ClobValue.Null(attributes),
                DiaType.Blob => BlobValue.Null(attributes),
                DiaType.List => ListValue.Null(attributes),
                DiaType.Record => RecordValue.Null(attributes),
                _ => throw new ArgumentException($"Invalid {typeof(DiaType)} value: {type}")
            };
        }
        #endregion

        #region Members
        /// <summary>
        /// The <see cref="DiaType"/>
        /// </summary>
        DiaType Type { get; }

        /// <summary>
        /// Indicating if the value is null (default).
        /// </summary>
        bool IsNull { get; }

        /// <summary>
        /// The attribute list
        /// </summary>
        Annotation[] Annotations { get; }
        #endregion
    }
}
