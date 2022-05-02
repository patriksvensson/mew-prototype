namespace Mew.CodeAnalysis.Interpreting;

public interface IExternalFunctionInvoker : IDisposable
{
    object? Invoke(ExternalFunctionSymbol symbol, ImmutableArray<object?> arguments);
}
