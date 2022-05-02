namespace Mew.CodeAnalysis.Semantics;

public class VariableSymbol : Symbol
{
    public TypeSymbol Type { get; }
    public override SymbolKind Kind { get; } = SymbolKind.Variable;

    public VariableSymbol(string name, TypeSymbol type)
        : base(name)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }
}
