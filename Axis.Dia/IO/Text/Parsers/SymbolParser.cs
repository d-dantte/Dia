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
    public class SymbolParser : IValueSerializer<SymbolValue>
    {
        #region Symbols
        public const string SymbolNameDiaSymbol = "dia-symbol";
        #endregion


        private SymbolParser() { }

        public static string GrammarSymbol => throw new NotImplementedException();

        public static IResult<SymbolValue> Parse(CSTNode symbolNode, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }


        public static IResult<string> Serialize(SymbolValue value, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }
    }
}
