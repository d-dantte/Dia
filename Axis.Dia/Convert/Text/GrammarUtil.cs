using Axis.Dia.Convert.Text.Pulsar.EscapeMatchers;
using Axis.Dia.Convert.Text.Pulsar.Rules;
using Axis.Dia.Convert.Text.Pulsar.Validators;
using Axis.Pulsar.Grammar.Language;
using Axis.Pulsar.Grammar.Language.Rules.CustomTerminals;
using Axis.Pulsar.Languages.xBNF;

namespace Axis.Dia.Convert.Text
{
    internal static class GrammarUtil
    {
        #region Custom Terminals
        public static readonly string BlockCommentString = "BlockCommentString";
        public static readonly string DiaIdentifier = "DiaIdentifier";
        public static readonly string SinglelineSQDString = "Singleline-SQDString";
        public static readonly string SinglelineDQDString = "Singleline-DQDString";
        public static readonly string MultilineDQDString = "Multiline-DQDString";
        public static readonly string BlobValue = "BlobValue";
        public static readonly string BlobTextValue = "blob-text-value";
        public static readonly string ClobValue = "ClobValue";
        #endregion

        public static Grammar Grammar { get; }

        static GrammarUtil()
        {
            using var xbnfStream = typeof(GrammarUtil).Assembly
                .GetManifestResourceStream($"{typeof(GrammarUtil).Namespace}.DiaGrammar.xbnf");

            var importer = new Importer();

            #region Register Terminals
            // register multiline-dqdstring
            _ = importer.RegisterTerminal(
                new DelimitedString(
                    MultilineDQDString,
                    "@\"", "\"",
                    new MultiLneStringEscapeMatcher()));

            // register singleline-sqdstring
            _ = importer.RegisterTerminal(
                new DelimitedString(
                    SinglelineDQDString,
                    "\"",
                    new[] { "\n", "\r" },
                    new SingleLneStringEscapeMatcher()));

            // register singleline-dqdstring
            _ = importer.RegisterTerminal(
                new DelimitedString(
                    SinglelineSQDString,
                    "\'",
                    new[] { "\n", "\r" },
                    new SingleLneStringEscapeMatcher()));

            // register clob-value
            _ = importer.RegisterTerminal(
                new DelimitedString(
                    ClobValue,
                    "<<", ">>",
                    new ClobEscapeMatcher()));

            // register blob-value
            _ = importer.RegisterTerminal(
                new DelimitedString(
                    BlobValue,
                    "<", ">",
                    Base64LegalCharacters(),
                    Array.Empty<string>()));

            // register block comment
            _ = importer.RegisterTerminal(
                new DelimitedString(
                    BlockCommentString,
                    "/*", "*/"));

            // register identifier rule
            _ = importer.RegisterTerminal(new SymbolIdentifierRule());
            #endregion

            #region Register Validators

            // register blob validator
            _ = importer.RegisterValidator(BlobTextValue, new BlobProductionValidator());

            #endregion

            Grammar = importer.ImportGrammar(xbnfStream);
        }


        private static char[] CharRange(char start, char end)
        {
            if (end < start)
                throw new ArgumentOutOfRangeException(nameof(end));

            return Enumerable
                .Range(start, (end - start) + 1)
                .Select(c => (char)c)
                .ToArray();
        }

        private static string[] Base64LegalCharacters()
        {
            return GrammarUtil
                .CharRange('A', 'Z')
                .Concat(CharRange('a', 'z'))
                .Concat(CharRange('0', '9'))
                .Append('+')
                .Append('=')
                .Append('/')
                .Append(' ')
                .Append('\t')
                .Append('\n')
                .Append('\r')
                .Select(c => c.ToString())
                .ToArray();
        }
    }
}
