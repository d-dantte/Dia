using Axis.Dia.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
