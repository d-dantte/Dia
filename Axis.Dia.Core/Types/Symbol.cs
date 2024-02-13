namespace Axis.Dia.Core.Types
{
    public readonly struct Symbol:
        IRefValue<string>,
        IEquatable<Symbol>,
        IDefaultContract<Symbol>
    {
    }
}
