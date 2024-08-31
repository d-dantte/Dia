using Axis.Pulsar.Core.CST;
using Axis.Pulsar.Core.Grammar;
using Axis.Pulsar.Core.Grammar.Validation;
using Axis.Pulsar.Core.Lang;
using Axis.Pulsar.Core.XBNF.Definitions;
using Axis.Pulsar.Core.XBNF.Lang;

namespace Axis.Dia.AxonSerializer.Deserializers
{
    internal static class GrammarUtil
    {
        private static readonly string SymbolNameBlobContent = "blob-content";
        public static ILanguageContext LanguageContext { get; }

        static GrammarUtil()
        {
            try
            {
                // get language string
                using var langDefStream = typeof(GrammarUtil).Assembly.GetManifestResourceStream(
                    "Axis.Dia.AxonSerializer.Deserializers.axon.xbnf");
                var langText = new StreamReader(langDefStream!).ReadToEnd();

                // build importer
                var importer = XBNFImporter
                    .NewBuilder()
                    .WithDefaultAtomicRuleDefinitions()
                    .WithProductionValidator(ProductionValidatorDefinition.Of(
                        SymbolNameBlobContent, new BlobContentValidator()))
                    .Build();

                // import
                LanguageContext = importer.ImportLanguage(langText);
            }
            catch (Exception)
            {
                throw;
            }
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

        private static char[] CharRange(char start, char end)
        {
            if (end < start)
                throw new ArgumentOutOfRangeException(nameof(end));

            return Enumerable
                .Range(start, (end - start) + 1)
                .Select(c => (char)c)
                .ToArray();
        }

        internal class BlobContentValidator : IProductionValidator
        {
            public Status Validate(
                SymbolPath symbolPath,
                ILanguageContext context,
                ISymbolNode recogniedNode)
            {
                return Status.Valid;
            }
        }
    }
}
