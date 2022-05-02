namespace Mew.CodeAnalysis.Semantics;

public abstract class FunctionSymbol : Symbol
{
    public IFunctionDeclarationSyntax Declaration { get; }
    public ImmutableArray<ParameterSymbol> Parameters { get; }
    public TypeSymbol ReturnType { get; }

    protected FunctionSymbol(
        string name,
        IFunctionDeclarationSyntax syntax,
        ImmutableArray<ParameterSymbol> parameters,
        TypeSymbol returnType)
            : base(name)
    {
        Declaration = syntax ?? throw new ArgumentNullException(nameof(syntax));
        Parameters = parameters;
        ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
    }
}
