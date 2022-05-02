namespace Mew.CodeAnalysis.Interpreting;

public sealed class InterpreterRuntimeException : Exception
{
    public Syntax Syntax { get; }

    public InterpreterRuntimeException(Syntax origin, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Syntax = origin;
    }
}
