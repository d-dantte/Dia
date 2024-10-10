namespace Axis.Dia.Bion
{
    public interface IDefaultInstance<TSelf> where TSelf : IDefaultInstance<TSelf>
    {
        public static abstract TSelf DefaultInstance { get; }
    }
}
