namespace Mew.CodeAnalysis.Semantics;

public class TypeSymbol : Symbol
{
    public override SymbolKind Kind { get; } = SymbolKind.Type;

    public static TypeSymbol Error { get; } = new TypeSymbol("#error");

    public TypeSymbol(string name)
        : base(name)
    {
    }
}
