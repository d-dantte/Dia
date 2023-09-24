using Axis.Dia.Contracts;

namespace Axis.Dia.Utils
{
    public static class DiaValueExtensions
    {
        public static bool HasAnnotations(this IDiaValue value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return value.Annotations.Length > 0;
        }
    }
}
