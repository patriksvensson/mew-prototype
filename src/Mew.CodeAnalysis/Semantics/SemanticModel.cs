namespace Mew.CodeAnalysis.Semantics;

public sealed class SemanticModel
{
    public string Name { get; }
    public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
    public BoundGlobalScope GlobalScope { get; }
    public Diagnostics Diagnostics { get; }
    public ImmutableDictionary<DeclaredFunctionSymbol, BoundBlockStatement> Functions { get; }
    public ImmutableArray<ExternalFunctionSymbol> ExternalFunctions { get; }
    public ImmutableArray<BoundStatement> Statements { get; }
    public SymbolCache SymbolCache { get; }

    public SemanticModel(
        string name,
        ImmutableArray<SyntaxTree> syntaxTrees,
        BoundGlobalScope globalScope,
        Diagnostics diagnostics,
        ImmutableDictionary<DeclaredFunctionSymbol, BoundBlockStatement> functions,
        ImmutableArray<ExternalFunctionSymbol> externalFunctions,
        ImmutableArray<BoundStatement> statements)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        SyntaxTrees = syntaxTrees;
        GlobalScope = globalScope ?? throw new ArgumentNullException(nameof(globalScope));
        Diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
        Functions = functions;
        ExternalFunctions = externalFunctions;
        Statements = statements;
        SymbolCache = new SymbolCache(this);
    }

    public static SemanticModel Create(string name, ImmutableArray<SyntaxTree> syntaxTrees)
    {
        // Create the global scope for lookup of global variables and functions
        var globalScope = BoundGlobalScope.Create(syntaxTrees);
        var context = globalScope.CreateBinderContext();

        // Bind function bodies
        var functionBodies = ImmutableDictionary.CreateBuilder<DeclaredFunctionSymbol, BoundBlockStatement>();
        var externalFunctions = ImmutableArray.CreateBuilder<ExternalFunctionSymbol>();
        foreach (var function in globalScope.Functions)
        {
            if (function is DeclaredFunctionSymbol declaredFunction &&
                function.Declaration is FunctionDeclarationStatement functionSyntax)
            {
                var functionContext = context.CreateForFunction(function);
                var body = Binder.BindStatement(functionContext, functionSyntax.Body);
                var lowered = Lowerer.Lower(body);

                functionBodies.Add(declaredFunction, Flattener.Flatten(lowered));
            }
            else if (function is ExternalFunctionSymbol externalFunction)
            {
                externalFunctions.Add(externalFunction);
            }
        }

        // Now bind all global statements
        var statements = ImmutableArray.CreateBuilder<BoundStatement>();
        foreach (var syntaxTree in syntaxTrees)
        {
            foreach (var statement in syntaxTree.Root.Statements)
            {
                // Skip function and type declarations since those already are bound
                if (statement is FunctionDeclarationStatement ||
                    statement is ExternalFunctionDeclarationStatement)
                {
                    continue;
                }

                var lowered = Lowerer.Lower(Binder.BindStatement(context, statement));
                statements.Add(lowered);
            }
        }

        // Return the bound program
        return new SemanticModel(
            name,
            syntaxTrees,
            globalScope,
            context.Diagnostics,
            functionBodies.ToImmutable(),
            externalFunctions.ToImmutable(),
            Flattener.Flatten(statements));
    }
}
