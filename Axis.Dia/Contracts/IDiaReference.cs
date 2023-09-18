namespace Axis.Dia.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDiaReference: IDiaValue
    {
        /// <summary>
        /// The address of the referred <see cref="IDiaReference"/> instance
        /// </summary>
         Guid ValueAddress { get; }

        /// <summary>
        /// The value from which the address is gotten. This may be null, in which case the reference is un-linked
        /// </summary>
         IDiaValue? Value { get; }

        /// <summary>
        /// Indicates if the value owning the address is present in this reference instance.
        /// </summary>
         bool IsLinked { get; }


        #region API

        /// <summary>
        /// Link the given <paramref name="addressableValue"/> to this reference, relocatingi the value if specified
        /// </summary>
        /// <typeparam name="TDiaValue">The type of the <see cref="IDiaReferable{TType}"/> instance.</typeparam>
        /// <param name="addressableValue">The instance to link to this reference</param>
        /// <returns>The linked value</returns>
        public IDiaReference LinkValue<TDiaValue>(IDiaReferable<TDiaValue> addressableValue)
        where TDiaValue : IDiaReferable<TDiaValue>, IDiaValue;
        #endregion
    }
}
