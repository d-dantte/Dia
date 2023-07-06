using Axis.Pulsar.Grammar.Language;
using Axis.Pulsar.Grammar.Language.Rules.CustomTerminals;
using Axis.Pulsar.Languages.xBNF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Axis.Pulsar.Grammar.Language.Rules.CustomTerminals.DelimitedString;

namespace Axis.Dia.IO.Text
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
        public static readonly string ClobValue = "ClobValue";
        #endregion

        public static Grammar Grammar { get; }

        static GrammarUtil()
        {
            using var xbnfStream = typeof(GrammarUtil).Assembly
                .GetManifestResourceStream($"{typeof(GrammarUtil).Namespace}.DiaGrammar.xbnf");

            var importer = new Importer();

            // register multiline-3sqdstring
            _ = importer.RegisterTerminal(
                new DelimitedString(
                    MultilineSingleQuoteDelimitedString,
                    "'''",
                    new BSolGeneralEscapeMatcher()));

            // register singleline-sqdstring
            _ = importer.RegisterTerminal(
                new DelimitedString(
                    SinglelineSingleQuoteDelimitedString,
                    "\'",
                    new[] { "\n", "\r" },
                    new BSolGeneralEscapeMatcher()));

            // register singleline-dqdstring
            _ = importer.RegisterTerminal(
                new DelimitedString(
                    SinglelineDoubleQuoteDelimitedString,
                    "\"",
                    new[] { "\n", "\r" },
                    new BSolGeneralEscapeMatcher()));

            Grammar = importer.ImportGrammar(ionXbnfStream);

        }
    }
}
