using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Language;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Pulsar.Grammar.Recognizers;
using Axis.Pulsar.Grammar;
using System.Text;
using Axis.Dia.Convert.Axon.Pulsar.Rules;
using Axis.Dia.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.Convert.Axon.Pulsar.Recognizers
{
    public class SymbolIdentifierRecognizer : IRecognizer
    {
        private HashSet<string> keywords = new HashSet<string>();

        public Grammar Grammar { get; }

        public IRule Rule { get; }

        public SymbolIdentifierRecognizer(Grammar grammar, IRule rule)
        {
            Grammar = grammar ?? throw new ArgumentNullException(nameof(grammar));
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
            keywords.AddRange(SymbolIdentifierRule.KeyWords);
        }

        public IRecognitionResult Recognize(BufferedTokenReader tokenReader)
        {
            _ = TryRecognize(tokenReader, out var result);
            return result;
        }

        public bool TryRecognize(
            BufferedTokenReader tokenReader,
            out IRecognitionResult result)
        {
            var position = tokenReader.Position;

            try
            {
                var sbuffer = new StringBuilder();
                var failureCount = 0;
                while (tokenReader.TryNextToken(out var token))
                {
                    if (!SymbolValue.IdentifierPattern.IsMatch(sbuffer.Append(token).ToString()))
                    {
                        if (++failureCount >= 2)
                        {
                            sbuffer.Remove(sbuffer.Length - 2, 2);
                            tokenReader.Back(2);
                            break;
                        }
                    }

                    else failureCount = 0;
                }

                var resultToken = sbuffer.ToString();
                if (SymbolValue.IdentifierPattern.IsMatch(resultToken)
                    && !keywords.Contains(resultToken))
                {
                    result = new SuccessResult(position + 1, CSTNode.Of(Rule.SymbolName, resultToken));
                    return true;
                }
                else
                {
                    result = new FailureResult(
                    position + 1,
                        IReason.Of(Rule.SymbolName));
                    tokenReader.Reset(position);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _ = tokenReader.Reset(position);
                result = new ErrorResult(position + 1, ex);
                return false;
            }
        }
    }
}
