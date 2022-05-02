namespace Mew.CodeAnalysis.Semantics;

public class DeclaredFunctionSymbol : FunctionSymbol
{
    public override SymbolKind Kind { get; } = SymbolKind.Function;

    public DeclaredFunctionSymbol(
        string name,
        IFunctionDeclarationSyntax syntax,
        ImmutableArray<ParameterSymbol> parameters,
        TypeSymbol returnType)
            : base(name, syntax, parameters, returnType)
    {
    }
}
