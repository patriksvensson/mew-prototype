namespace Mew.CodeAnalysis.Semantics;

internal sealed class BinderContext
{
    private readonly Stack<BoundScope> _scopes;
    private readonly Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)> _loopStack;
    private int _labelCounter;

    public FunctionSymbol? Function { get; }
    public Diagnostics Diagnostics { get; }

    public BoundScope Scope => _scopes.Peek();

    public bool IsInLoop => _loopStack.Count > 0;
    public BoundLabel BreakLabel => _loopStack.Peek().BreakLabel;
    public BoundLabel ContinueLabel => _loopStack.Peek().ContinueLabel;

    public BinderContext(BoundScope? scope, FunctionSymbol? function, Diagnostics? diagnostics)
    {
        _scopes = new Stack<BoundScope>();
        _scopes.Push(new BoundScope(scope));

        _loopStack = new Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)>();

        Function = function;
        Diagnostics = diagnostics ?? new Diagnostics();

        // Add function parameters as variables
        if (Function != null)
        {
            foreach (var parameter in Function.Parameters)
            {
                Scope.TryDeclareVariable(parameter);
            }
        }
    }

    public void AddDiagnostic(Syntax syntax, DiagnosticDescriptor descriptor)
    {
        // TODO: Get the location
        var location = new Location(syntax.SyntaxTree.Path, syntax.Span);
        Diagnostics.Add(descriptor.ToDiagnostic(location));
    }

    public BinderContext CreateForFunction(FunctionSymbol function)
    {
        return new BinderContext(Scope, function, Diagnostics);
    }

    public void RunInChildScope(Action action)
    {
        _scopes.Push(new BoundScope(Scope));

        try
        {
            action();
        }
        finally
        {
            _scopes.Pop();
        }
    }

    public BoundStatement BindLoop(
        BinderContext context,
        Syntax body,
        out BoundLabel breakLabel,
        out BoundLabel continueLabel)
    {
        _labelCounter++;

        breakLabel = new BoundLabel($"break{_labelCounter}");
        continueLabel = new BoundLabel($"continue{_labelCounter}");

        _loopStack.Push((breakLabel, continueLabel));
        var boundBody = Binder.BindStatement(context, body);
        _loopStack.Pop();

        return boundBody;
    }
}
