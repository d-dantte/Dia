using Axis.Dia.Core.Contracts;

namespace Axis.Dia.TypeConverter
{
    public readonly struct TypeInfo: IDefaultContract<TypeInfo>
    {
        public TypeCategory Category { get; }

        public Type Type { get; }

        public Type? ItemType { get; }

        public static TypeInfo Default => default;

        public bool IsDefault => Category == default && Type is null;

        internal TypeInfo(
            TypeCategory category,
            Type type)
            : this(category, type, null)
        { }

        internal TypeInfo(TypeCategory category, Type type, Type? itemType)
        {
            ArgumentNullException.ThrowIfNull(type);

            Type = type;
            ItemType = itemType;
            Category = category;
        }

        public static TypeInfo Of(Type type)
            => new(type.ToTypeCategory(out var itemType), type, itemType);

        public static TypeInfo Of(TypeCategory category, Type type) => new(category, type);

        public static TypeInfo Of(
            TypeCategory category,
            Type type,
            Type? itemType)
            => new(category, type, itemType);

        public override string? ToString()
        {
            // TODO: add a proper string representation of this type
            return base.ToString();
        }
    }
}
