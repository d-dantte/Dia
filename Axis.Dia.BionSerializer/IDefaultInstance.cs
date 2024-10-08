namespace Axis.Dia.BionSerializer
{
    public interface IDefaultInstance<TSelf> where TSelf : IDefaultInstance<TSelf>
    {
        public static abstract TSelf DefaultInstance { get; }
    }
}
