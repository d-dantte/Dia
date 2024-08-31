using Axis.Pulsar.Core.Lang;
using Axis.Pulsar.Core.XBNF.Lang;

namespace Axis.Dia.PathQuery.Grammar
{
    internal class GrammarUtil
    {
        public static ILanguageContext LanguageContext { get; }

        static GrammarUtil()
        {
            try
            {
                // get language string
                using var xbnfStream = typeof(GrammarUtil).Assembly.GetManifestResourceStream(
                    $"{typeof(GrammarUtil).Namespace}.dpql.xbnf");

                var langText = new StreamReader(xbnfStream!).ReadToEnd();

                // build importer
                var importer = XBNFImporter
                    .NewBuilder()
                    .WithDefaultAtomicRuleDefinitions()
                    .Build();

                // import
                LanguageContext = importer.ImportLanguage(langText);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
