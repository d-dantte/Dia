using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;

namespace Axis.Dia.PathQuery.Accessors
{
    public readonly struct PropertyAccessor :
        IAccessor<Record, Record.PropertyName>,
        IDefaultContract<PropertyAccessor>
    {
        public static PropertyAccessor Default => default;

        public bool IsDefault => Source.IsDefault && Key.IsDefault;

        public Record Source{ get; }

        public Record.PropertyName Key { get; }

        public PropertyAccessor(Record source, Record.PropertyName key)
        {
            Source = source;
            Key = key;
        }
    }
}
