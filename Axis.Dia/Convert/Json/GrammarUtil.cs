using Axis.Dia.Convert.Json.Pulsar.EscapeMatchers;
using Axis.Dia.Convert.Json.Pulsar.Rules;
using Axis.Pulsar.Grammar.Language;
using Axis.Pulsar.Grammar.Language.Rules.CustomTerminals;
using Axis.Pulsar.Languages.xBNF;

namespace Axis.Dia.Convert.Json
{
    internal static class GrammarUtil
    {
        #region Custom Terminals
        public static readonly string SingleQuoteText = "SQText";
        public static readonly string Identifier = "Identifier";
        public static readonly string StringContent = "StringContent";
        #endregion

        public static Grammar Grammar { get; }

        static GrammarUtil()
        {
            using var xbnfStream = typeof(GrammarUtil).Assembly
                .GetManifestResourceStream($"{typeof(GrammarUtil).Namespace}.JsonGrammar.xbnf");

            var importer = new Importer();

            // register SQText
            _ = importer.RegisterTerminal(
                new DelimitedString(
                    symbolName: SingleQuoteText,
                    delimiter: "\'",
                    illegalSequences: new[] { "\n", "\r" },
                    escapeMatchers: new SQTextEscapeMatcher()));

            // register identifier
            _ = importer.RegisterTerminal(new IdentifierRule());

            // register string content
            _ = importer.RegisterTerminal(new StringContentRule());

            Grammar = importer.ImportGrammar(xbnfStream);
        }
    }
}
