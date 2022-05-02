namespace Mew.CodeAnalysis.Semantics;

public class ExternalFunctionSymbol : FunctionSymbol
{
    public string Library { get; }

    public override SymbolKind Kind { get; } = SymbolKind.ExternalFunction;

    public ExternalFunctionSymbol(
        string name,
        IFunctionDeclarationSyntax syntax,
        ImmutableArray<ParameterSymbol> parameters,
        TypeSymbol returnType,
        string library)
            : base(name, syntax, parameters, returnType)
    {
        Library = library ?? throw new ArgumentNullException(nameof(library));
    }
}
