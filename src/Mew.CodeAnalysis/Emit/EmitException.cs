namespace Mew.CodeAnalysis.Emit;

public sealed class EmitException : Exception
{
    public EmitException(string message)
        : base(message)
    {
    }
}
