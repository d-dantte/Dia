using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Language;
using Axis.Pulsar.Grammar.Recognizers.Results;
using Axis.Pulsar.Grammar.Recognizers;
using Axis.Pulsar.Grammar;
using System.Text;
using System.Text.RegularExpressions;

namespace Axis.Dia.Convert.Json.Pulsar.Recognizers
{
    public class StringContentRecognizer : IRecognizer
    {
        private readonly Regex Hex4Pattern = new Regex("^u[a-fA-F0-9]{4}$", RegexOptions.Compiled);

        public Grammar Grammar { get; }

        public IRule Rule { get; }

        public StringContentRecognizer(Grammar grammar, IRule rule)
        {
            Grammar = grammar ?? throw new ArgumentNullException(nameof(grammar));
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
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
                var hasInvalidEscape = false;
                while (tokenReader.TryNextToken(out var token))
                {
                    if ('"'.Equals(token))
                    {
                        tokenReader.Back(1);
                        break;
                    }
                    else if ('\\'.Equals(token))
                    {
                        if (TryRecognizeEscapeSequence(tokenReader, out var escapeSequence))
                            _ = sbuffer.Append('\\').Append(escapeSequence);

                        else
                        {
                            hasInvalidEscape = true;
                            break;
                        }
                    }
                    else sbuffer.Append(token);
                }

                if (!hasInvalidEscape)
                {
                    result = new SuccessResult(position + 1, CSTNode.Of(Rule.SymbolName, sbuffer.ToString()));
                    return true;
                }
                else
                {
                    result = new FailureResult(
                        tokenReader.Position,
                        IReason.Of("EscapeSequence"));
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

        private bool TryRecognizeEscapeSequence(BufferedTokenReader tokenReader, out string escapeSequence)
        {
            var position = tokenReader.Position;

            // utf-hex4
            if (tokenReader.Reset(position).TryNextTokens(5, out var tokens)
                && Hex4Pattern.IsMatch(new string(tokens)))
            {
                escapeSequence = new string(tokens);
                return true;
            }

            if (tokenReader.Reset(position).TryNextTokens(1, out tokens)
                && IsBasicEscapeArg(tokens[0]))
            {
                escapeSequence = new string(tokens);
                return true;
            }

            tokenReader.Reset(position);
            escapeSequence = string.Empty;
            return false;
        }

        private static bool IsBasicEscapeArg(char c)
        {
            return c switch
            {
                '\"' => true,
                '\\' => true,
                '/' => true,
                'b' => true,
                'f' => true,
                'n' => true,
                'r' => true,
                't' => true,
                _ => false,
            };
        }
    }
}
