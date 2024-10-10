namespace Axis.Dia.Typhon
{
    internal interface IDefaultInstance<TSelf> where  TSelf : IDefaultInstance<TSelf>
    {
        public abstract static TSelf DefaultInstance { get; }
    }
}
