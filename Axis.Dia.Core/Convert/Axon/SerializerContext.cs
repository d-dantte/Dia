using Axis.Dia.Core.Convert.Axon.Serializers;

namespace Axis.Dia.Core.Convert.Axon
{
    public readonly struct SerializerContext: IDefaultContract<SerializerContext>
    {
        #region DefaultContract
        public static SerializerContext Default => default;

        public bool IsDefault
            => Options is null
            && Depth == 0;
        #endregion

        public Options Options { get; }

        public ushort Depth { get; }

        public SerializerContext(Options options, ushort depth = 0)
        {
            ArgumentNullException.ThrowIfNull(options);

            Options = options;
            Depth = depth;
        }

        public static SerializerContext Of(
            Options options,
            ushort depth = 0)
            => new(options, depth);

        public string Indent() => Options.Indentation switch
        {
            Options.IndentationStyle.Spaces => "".PadLeft(Depth * 4),
            Options.IndentationStyle.Tabs => "".PadLeft(Depth, '\t'),
            _ => ""
        };

        /// <summary>
        /// Gets a "new" context representing <paramref name="level"/> levels of additional depth.
        /// </summary>
        /// <returns></returns>
        public SerializerContext Next(ushort level = 1)
        {
            return new SerializerContext(Options, (ushort) (Depth + level));
        }
    }
}
