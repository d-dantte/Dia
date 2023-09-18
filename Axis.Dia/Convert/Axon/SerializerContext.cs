using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Extensions;
using System.Text;
using static Axis.Dia.Convert.Axon.SerializerOptions;

namespace Axis.Dia.Convert.Axon
{   
    public class SerializerContext
    {
        private readonly Dictionary<Guid, RefInfo> refDataMap = new();

        public SerializerOptions Options { get; }

        public int IndentationLevel { get; }

        internal SerializerContext(
            SerializerOptions options,
            ushort indentationLevel,
            Dictionary<Guid, RefInfo>? refDataMap)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            IndentationLevel = indentationLevel;
            this.refDataMap.AddRange(refDataMap ?? new Dictionary<Guid, RefInfo>());
        }

        internal SerializerContext(
            SerializerOptions options,
            ushort indentationLevel)
        : this(options, indentationLevel, null)
        {
        }

        internal SerializerContext(SerializerOptions options)
        : this(options, 0)
        {
        }

        internal SerializerContext()
        : this(new SerializerOptions(), 0)
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

        public SerializerContext IndentContext(ushort additionalIndentationLevels = 0)
        {
            return new SerializerContext(
                indentationLevel: (ushort)(additionalIndentationLevels + IndentationLevel + 1),
                refDataMap: refDataMap,
                options: Options);
        }

        internal bool TryGetRefInfo(IDiaReference @ref, out RefInfo? refInfo)
        {
            return refDataMap.TryGetValue(@ref.ValueAddress, out refInfo);
        }

        internal void TrackReferences(IDiaValue value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            else if (value is ReferenceValue @ref)
                refDataMap[@ref.ValueAddress] = new RefInfo(refDataMap.Count + 1);

            else if (value is ListValue list && !list.IsNull)
                list.Value!.ForAll(TrackReferences);

            else if (value is RecordValue record && !record.IsNull)
                record.Values!.ForAll(TrackReferences);
        }

        #region Nested data
        public class RefInfo
        {
            public int Index { get; }

            public bool IsSerialized { get; private set; }

            public void Serialized() => IsSerialized = true;

            public RefInfo(int index)
            {
                Index = index; 
            } 
        }
        #endregion
    }

    public class ParserContext
    {
        private readonly Dictionary<int, Guid> addressMap = new();

        internal Guid AllocateAddress(int refIndex)
        {
            return addressMap[refIndex] = Guid.NewGuid();
        }

        internal bool TryGetRefAddress(int refIndex, out Guid address)
        {
            return addressMap.TryGetValue(refIndex, out address);
        }
    }
}
