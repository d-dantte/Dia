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
    public class DecimalParser : IValueSerializer<DecimalValue>
    {
        #region Symbols
        public const string SymbolNameDiaDecimal = "dia-decimal";
        #endregion


        private DecimalParser() { }

        public static string GrammarSymbol => SymbolNameDiaDecimal;

        public static IResult<DecimalValue> Parse(CSTNode symbolNode, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }


        public static IResult<string> Serialize(DecimalValue value, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }
    }
}
