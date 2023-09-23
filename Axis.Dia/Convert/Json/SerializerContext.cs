﻿using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Common;
using Axis.Luna.Extensions;

namespace Axis.Dia.Convert.Json
{
    public readonly struct SerializerContext : IDefaultValueProvider<SerializerContext>
    {
        private readonly Dictionary<Guid, int> addressIndexMap = new();

        public SerializerOptions Options { get; }

        public int IndentationLevel { get; }

        #region
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

        internal void BuildAddressIndices(IDiaValue value)
        {
            var map = addressIndexMap;

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            else if (value is ReferenceValue @ref)
                _ = addressIndexMap.GetOrAdd(@ref.ValueAddress, _ => map.Count + 1);

            else if (value is ListValue list && !list.IsNull)
                list.Value!.ForAll(BuildAddressIndices);

            else if (value is RecordValue record && !record.IsNull)
                record.Values!.ForAll(BuildAddressIndices);
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
}
