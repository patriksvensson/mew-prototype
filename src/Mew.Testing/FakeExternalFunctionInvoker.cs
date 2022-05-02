using System.Collections.Immutable;
using Mew.CodeAnalysis.Interpreting;
using Mew.CodeAnalysis.Semantics;

namespace Mew.Testing;

public sealed class FakeExternalFunctionInvoker : IExternalFunctionInvoker
{
    private readonly Dictionary<string, ImmutableArray<object?>> _received;

    public FakeExternalFunctionInvoker()
    {
        _received = new Dictionary<string, ImmutableArray<object?>>();
    }

    public void Dispose()
    {
    }

    public object? Invoke(ExternalFunctionSymbol symbol, ImmutableArray<object?> arguments)
    {
        _received[symbol.Name] = arguments;
        return null;
    }

    public bool ReceivedCall(string name)
    {
        return _received.ContainsKey(name);
    }
}
