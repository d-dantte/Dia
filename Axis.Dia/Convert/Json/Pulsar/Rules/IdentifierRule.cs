using Axis.Pulsar.Grammar.Language.Rules.CustomTerminals;
using Axis.Pulsar.Grammar.Language;
using Axis.Pulsar.Grammar.Recognizers;
using Axis.Dia.Convert.Json.Pulsar.Recognizers;

namespace Axis.Dia.Convert.Json.Pulsar.Rules
{
    public class IdentifierRule : ICustomTerminal
    {
        public static string[] KeyWords => _keyWords.ToArray();

        private static readonly string[] _keyWords = new string[]
        {
            "null", "true", "false"
        };

        public string SymbolName => "Identifier";

        public IRecognizer ToRecognizer(Grammar grammar) => new IdentifierRecognizer(grammar, this);
    }
}
