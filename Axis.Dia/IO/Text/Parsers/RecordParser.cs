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
    public class RecordParser : IValueSerializer<RecordValue>
    {
        #region Symbols
        public const string SymbolNameDiaRecord = "dia-record";
        #endregion


        private RecordParser() { }

        public static string GrammarSymbol => throw new NotImplementedException();

        public static IResult<RecordValue> Parse(CSTNode symbolNode, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }


        public static IResult<string> Serialize(RecordValue value, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }
    }
}
