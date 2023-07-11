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
    public class StringParser : IValueSerializer<StringValue>
    {
        #region Symbols
        public const string SymbolNameDiaString = "dia-string";
        #endregion


        private StringParser() { }

        public static string GrammarSymbol => throw new NotImplementedException();

        public static IResult<StringValue> Parse(CSTNode symbolNode, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }


        public static IResult<string> Serialize(StringValue value, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }
    }
}
