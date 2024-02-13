//using static Axis.Dia.Core.Types.Record;

namespace Axis.Dia.Core.Types
{
    public readonly struct Record :
        IValueContainer<Record, Record.Property>,
        IDefaultContract<Attribute>,
        IEquatable<Attribute>
    {
        private readonly Dictionary<string, (Symbol, IDiaValue)> _values = new();


        #region Map Api

        public ValueWrapper this[Symbol propertyKey]
        {
            get
            {

            }
            set
            {

            }
        }

        #endregion


        #region Nested types
        public readonly struct Property
        {

        }
        #endregion
    }
}
