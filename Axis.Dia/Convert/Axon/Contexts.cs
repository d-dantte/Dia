using Axis.Dia.Contracts;
using Axis.Luna.Extensions;

namespace Axis.Dia.Convert.Axon
{
    public readonly struct SerializerContext
    {
        private readonly Dictionary<Guid, int> addressIndexMap = new();

        public SerializerOptions Options { get; }

        public int IndentationLevel { get; }

        #region DefaultProvider
        public bool IsDefault => Default.Equals(this);

        public static SerializerContext Default => default;
        #endregion

        #region Construction
        public SerializerContext()
        : this(SerializerOptionsBuilder.NewBuilder().Build())
        {
        }

        internal SerializerContext(SerializerOptions options)
        : this(options, 0)
        {
        }

        internal SerializerContext(
            SerializerOptions options,
            ushort indentationLevel)
            : this(options, indentationLevel, null)
        {
        }

        internal SerializerContext(
            SerializerOptions options,
            ushort indentationLevel,
            Dictionary<Guid, int>? addressIndexMap)
        {
            Options = options.ThrowIfDefault(new ArgumentException("Invalid options supplied"));
            IndentationLevel = indentationLevel;
            this.addressIndexMap = addressIndexMap ?? new();
        }
        #endregion

        #region API
        internal bool TryGetAddressIndex(IDiaAddressProvider addressProvider, out int index)
        {
            return addressIndexMap.TryGetValue(addressProvider.Address, out index);
        }

        internal void BuildAddressIndices(IEnumerable<IDiaReference> references)
        {
            var map = addressIndexMap;
            references.ForAll(@ref => map.GetOrAdd(@ref.ValueAddress, _ => map.Count + 1));
        }

        internal SerializerContext Indent(ushort newLevel)
        {
            return new SerializerContext(
                Options,
                newLevel,
                addressIndexMap);
        }

        internal SerializerContext Indent() => Indent((ushort)(IndentationLevel + 1));

        internal string IndentText(string text, ushort additionalIndentationLevels = 0)
        {
            var indentation = Options.IndentationStyle switch
            {
                SerializerOptions.IndentationStyles.Tabs => "".PadLeft(IndentationLevel + additionalIndentationLevels, '\t'),
                SerializerOptions.IndentationStyles.Spaces => "".PadLeft((IndentationLevel + additionalIndentationLevels) * 4, ' '),
                _ => ""
            };

            return $"{indentation}{text}";
        }
        #endregion
    }

    public readonly struct ParserContext
    {
        private readonly Dictionary<int, Guid> addressIndexMap = new();

        public ParserContext()
        {
        }

        internal Guid Track(int addressIndex)
        {
            return addressIndexMap.GetOrAdd(addressIndex, index => Guid.NewGuid());
        }
    }
}
