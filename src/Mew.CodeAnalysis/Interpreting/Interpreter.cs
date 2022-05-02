namespace Mew.CodeAnalysis.Interpreting;

public sealed class Interpreter
{
    private readonly IExternalFunctionInvoker _invoker;

    public Interpreter()
        : this(new ExternalFunctionInvoker())
    {
    }

    public Interpreter(IExternalFunctionInvoker invoker)
    {
        _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
    }

    public object? Evaluate(SemanticModel model)
    {
        if (model.Diagnostics.HasErrors)
        {
            throw new InvalidOperationException("Cannot run code with errors");
        }

        var context = new InterpreterContext(model, _invoker);
        return InterpreterVisitor.Shared.VisitStatements(model.Statements, context);
    }
}
