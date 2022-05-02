namespace Mew.CodeAnalysis.Semantics;

public sealed class UndeclaredTypeSymbol : TypeSymbol
{
    public UndeclaredTypeSymbol(string name)
        : base(name)
    {
    }
}