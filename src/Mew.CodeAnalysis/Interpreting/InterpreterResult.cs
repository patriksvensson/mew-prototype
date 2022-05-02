namespace Mew.CodeAnalysis.Interpreting;

public sealed class InterpreterResult
{
    public object? Result { get; }
    public Diagnostics Diagnostics { get; }
    public bool Success => !Diagnostics.HasErrors;

    public InterpreterResult(object? result, Diagnostics diagnostics)
    {
        Result = result;
        Diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
    }
}