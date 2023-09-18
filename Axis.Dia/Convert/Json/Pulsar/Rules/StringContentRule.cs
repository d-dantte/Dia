using Axis.Pulsar.Grammar.Language.Rules.CustomTerminals;
using Axis.Pulsar.Grammar.Language;
using Axis.Pulsar.Grammar.Recognizers;
using Axis.Dia.Convert.Json.Pulsar.Recognizers;

namespace Axis.Dia.Convert.Json.Pulsar.Rules
{
    public class StringContentRule : ICustomTerminal
    {
        public string SymbolName => "StringContent";

        public IRecognizer ToRecognizer(Grammar grammar) => new StringContentRecognizer(grammar, this);
    }
}
