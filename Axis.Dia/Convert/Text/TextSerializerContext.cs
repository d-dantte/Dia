using Axis.Dia.Utils;
using System.Text;
using static Axis.Dia.Convert.Text.TextSerializerOptions;

namespace Axis.Dia.Convert.Text
{
    public class TextSerializerContext
    {
        public TextSerializerOptions Options { get; }

        public int IndentationLevel { get; }

        public TextSerializerContext(
            TextSerializerOptions options,
            ushort indentationLevel)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            IndentationLevel = indentationLevel;
        }

        public TextSerializerContext(TextSerializerOptions options)
        : this(options, 0)
        {
        }

        public TextSerializerContext()
        : this(new TextSerializerOptions(), 0)
        {
        }

        public string Indentation(ushort additionalIndentationLevels = 0)
        {
            var indentation = Options.IndentationStyle switch
            {
                IndentationStyles.None => "",
                IndentationStyles.Spaces => "    ",
                IndentationStyles.Tabs => "\t",
                _ => throw new ArgumentException($"Invalid indentation style: {Options.IndentationStyle}")
            };

            if (string.Empty.Equals(indentation))
                return string.Empty;

            var sb = new StringBuilder();
            for (int cnt = 0; cnt < additionalIndentationLevels + IndentationLevel; cnt++)
            {
                sb.Append(indentation);
            }
            return sb.ToString();
        }

        public TextSerializerContext IndentContext(ushort additionalIndentationLevels = 0)
        {
            return new TextSerializerContext(
                indentationLevel: (ushort)(additionalIndentationLevels + IndentationLevel + 1),
                options: Options);
        }
    }
}
