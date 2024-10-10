using Axis.Dia.Core.Contracts;
using System.Numerics;

namespace Axis.Dia.Bion.Deserializers.Contracts
{
    public interface IValueTracker
    {
        TDiaValue Track<TDiaValue>(BigInteger streamIndex, TDiaValue value) where TDiaValue : IDiaValue;

        bool TryGet<TDiaValue>(BigInteger streamIndex, out TDiaValue? value) where TDiaValue : IDiaValue;

        TDiaValue GetValue<TDiaValue>(BigInteger streamIndex) where TDiaValue : IDiaValue;

        IDiaValue GetValue(BigInteger streamIndex);
    }
}
