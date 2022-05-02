namespace Mew.CodeAnalysis.Interpreting;

internal sealed class InterpreterContext
{
    public SemanticModel Model { get; }
    public IExternalFunctionInvoker Invoker { get; }
    public Dictionary<VariableSymbol, object?> Globals { get; }
    public Stack<Dictionary<VariableSymbol, object?>> Stack { get; }
    public Syntax? Current { get; set; }
    public object? LastValue { get; set; }

    public InterpreterContext(SemanticModel model, IExternalFunctionInvoker invoker)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
        Globals = new Dictionary<VariableSymbol, object?>();
        Stack = new Stack<Dictionary<VariableSymbol, object?>>();
    }
}
