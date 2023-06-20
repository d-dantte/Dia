namespace Axis.Dia.Contracts
{
    /// <summary>
    /// Defines deep-copy contract
    /// </summary>
    /// <typeparam name="TIon"></typeparam>
    public interface IDeepCopyable<TIon>
    where TIon : IDeepCopyable<TIon>
    {
        TIon DeepCopy();
    }
}
