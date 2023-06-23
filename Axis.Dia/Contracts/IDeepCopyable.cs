namespace Axis.Dia.Contracts
{
    /// <summary>
    /// Defines deep-copy contract
    /// </summary>
    /// <typeparam name="TIon"></typeparam>
    public interface IDeepCopyable<TType>
    where TType : IDeepCopyable<TType>
    {
        TType DeepCopy();
    }
}
