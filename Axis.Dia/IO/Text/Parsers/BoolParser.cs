using System;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.IO.Text.Parsers
{
    public class BoolParser : IValueSerializer<BoolValue>
    {
        private BoolParser() { }


        public static IResult<BoolValue> Parse(string text, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }


        public static IResult<string> Serialize(BoolValue value, TextSerializerContext? context = null)
        {
            throw new NotImplementedException();
        }
    }
}

