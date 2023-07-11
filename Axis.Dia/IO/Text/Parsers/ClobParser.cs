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
    public class ClobParser : IValueSerializer<ClobValue>
    {
        #region Symbols
        public const string SymbolNameDiaClob = "dia-clob";
        #endregion


        private ClobParser() { }

        public static string GrammarSymbol => throw new NotImplementedException();

        public static IResult<ClobValue> Parse(CSTNode symbolNode, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }


        public static IResult<string> Serialize(ClobValue value, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }
    }
}
