using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Dia.IO.Text.Parsers
{
    public class InstantParser : IValueSerializer<InstantValue>
    {
        #region Symbols
        public const string SymbolNameDiaInstant = "dia-instant";
        #endregion


        private InstantParser() { }

        public static string GrammarSymbol => throw new NotImplementedException();

        public static IResult<InstantValue> Parse(CSTNode symbolNode, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }


        public static IResult<string> Serialize(InstantValue value, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }
    }
}
