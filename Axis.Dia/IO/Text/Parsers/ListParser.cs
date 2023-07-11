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
    public class ListParser : IValueSerializer<ListValue>
    {
        #region Symbols
        public const string SymbolNameDiaList = "dia-list";
        #endregion


        private ListParser() { }

        public static string GrammarSymbol => throw new NotImplementedException();

        public static IResult<ListValue> Parse(CSTNode symbolNode, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }


        public static IResult<string> Serialize(ListValue value, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }
    }
}
