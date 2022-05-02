namespace Mew.CodeAnalysis.Semantics;

public sealed class ParameterSymbol : VariableSymbol
{
    public Syntax Declaration { get; }
    public override SymbolKind Kind { get; } = SymbolKind.Parameter;

    public ParameterSymbol(string name, Syntax syntax, TypeSymbol type)
        : base(name, type)
    {
        Declaration = syntax ?? throw new ArgumentNullException(nameof(syntax));
    }
}