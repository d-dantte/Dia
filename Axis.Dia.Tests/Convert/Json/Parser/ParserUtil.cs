using Axis.Dia.Convert.Json;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;

namespace Axis.Dia.Tests.Convert.Json.Parser
{
    public static class ParserUtil
    {
        public static CSTNode ParseTokens(string recognizer, string tokens)
        {
            var result = GrammarUtil.Grammar
                .GetRecognizer(recognizer)
                .Recognize(tokens);

            if (result is SuccessResult success)
                return success.Symbol;

            else throw new Pulsar.Grammar.Exceptions.RecognitionException(result);
        }
    }
}
