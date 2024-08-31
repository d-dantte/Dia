using Axis.Dia.Axon.Serializers;
using Axis.Dia.AxonSerializer;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Axon
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

        public ReferenceMap ReferenceMap { get; }

        public SerializerContext(
            Options options,
            ushort depth = 0)
            : this(options, new ReferenceMap(), depth)
        {
        }

        private SerializerContext(Options options, ReferenceMap referenceMap, ushort depth)
        {
            ArgumentNullException.ThrowIfNull(options);

            Options = options;
            Depth = depth;
            ReferenceMap = referenceMap;
        }

        public static SerializerContext Of(
            Options options,
            ushort depth)
            => new(options, depth);

        public static SerializerContext Of(
            Options options)
            => Of(options, 0);

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
            return new SerializerContext(Options, ReferenceMap, (ushort) (Depth + level));
        }
    }
}
