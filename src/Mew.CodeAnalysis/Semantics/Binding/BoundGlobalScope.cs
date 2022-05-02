namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundGlobalScope
{
    public BoundGlobalScope? Previous { get; }
    public Diagnostics Diagnostics { get; }
    public ImmutableArray<FunctionSymbol> Functions { get; }
    public ImmutableArray<VariableSymbol> Variables { get; }
    public ImmutableArray<TypeSymbol> Types { get; }

    private BoundGlobalScope(
        BoundGlobalScope? previous,
        Diagnostics diagnostics,
        ImmutableArray<TypeSymbol> types,
        ImmutableArray<FunctionSymbol> functions,
        ImmutableArray<VariableSymbol> variables)
    {
        Previous = previous;
        Diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
        Types = types;
        Functions = functions;
        Variables = variables;
    }

    public static BoundGlobalScope Create(ImmutableArray<SyntaxTree> syntaxTrees)
    {
        var context = new BinderContext(new BoundScope(null), function: null, diagnostics: Diagnostics.Create(syntaxTrees));

        // Bind function declarations
        foreach (var functionDeclaration in syntaxTrees
            .SelectMany(t => t.Root.Statements)
            .OfType<FunctionDeclarationStatement>())
        {
            Binder.BindFunctionDeclaration(context, functionDeclaration);
        }

        // Bind external function declarations
        foreach (var functionDeclaration in syntaxTrees
            .SelectMany(t => t.Root.Statements)
            .OfType<ExternalFunctionDeclarationStatement>())
        {
            Binder.BindExternalFunctionDeclaration(context, functionDeclaration);
        }

        return new BoundGlobalScope(
            previous: null,
            context.Diagnostics,
            context.Scope.Types.GetDeclared(),
            context.Scope.Functions.GetDeclared(),
            context.Scope.GetDeclaredVariables());
    }

    internal BinderContext CreateBinderContext()
    {
        var scope = new BoundScope(null);

        foreach (var function in Functions)
        {
            scope.Functions.DeclareFunction(function);
        }

        foreach (var variable in Variables)
        {
            scope.TryDeclareVariable(variable);
        }

        return new BinderContext(scope, function: null, Diagnostics);
    }
}
