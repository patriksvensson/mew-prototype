namespace Mew.CodeAnalysis.Semantics;

public sealed class FunctionSymbolTable
{
    private readonly Dictionary<string, HashSet<FunctionSymbol>> _functions;

    public FunctionSymbolTable()
    {
        _functions = new Dictionary<string, HashSet<FunctionSymbol>>();
    }

    public bool DeclareFunction(FunctionSymbol function)
    {
        if (!_functions.ContainsKey(function.Name))
        {
            _functions.Add(function.Name, new HashSet<FunctionSymbol>(new FunctionSymbolComparer()));
        }

        if (_functions[function.Name].Contains(function))
        {
            return false;
        }

        return _functions[function.Name].Add(function);
    }

    public bool HasFunction(string name)
    {
        return _functions.TryGetValue(name, out var _);
    }

    public ImmutableArray<FunctionSymbol> GetDeclared()
    {
        return _functions.SelectMany(f => f.Value).ToImmutableArray();
    }

    public bool TryGetExact(
        string name,
        ImmutableArray<BoundExpression> parameters,
        [NotNullWhen(true)] out FunctionSymbol? symbol)
    {
        if (_functions.TryGetValue(name, out var functions))
        {
            foreach (var function in functions)
            {
                if (function.Parameters.Length != parameters.Length)
                {
                    continue;
                }

                var isMatch = true;
                for (var index = 0; index < function.Parameters.Length; index++)
                {
                    if (function.Parameters[index].Type != parameters[index].Type)
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (!isMatch)
                {
                    continue;
                }

                symbol = function;
                return true;
            }
        }

        symbol = null;
        return false;
    }

    public bool TryGetClosest(
        string name,
        ImmutableArray<BoundExpression> parameters,
        [NotNullWhen(true)] out FunctionSymbol? symbol)
    {
        if (TryGetExact(name, parameters, out symbol))
        {
            return true;
        }

        if (_functions.TryGetValue(name, out var functions))
        {
            // Got something close?
            foreach (var function in functions)
            {
                if (function.Parameters.Length == parameters.Length)
                {
                    symbol = function;
                    return true;
                }
            }

            // Just return the first best function
            var anyFunction = functions.FirstOrDefault();
            if (anyFunction != null)
            {
                symbol = anyFunction;
                return true;
            }
        }

        symbol = null;
        return false;
    }

    internal bool TryGetWithArity(string name, int count, [NotNullWhen(true)] out FunctionSymbol? symbol)
    {
        if (_functions.TryGetValue(name, out var functions))
        {
            // Just return the first best function
            var anyFunction = functions.FirstOrDefault(x => x.Parameters.Length == count);
            if (anyFunction != null)
            {
                symbol = anyFunction;
                return true;
            }
        }

        symbol = null;
        return false;
    }

    public bool TryGetAny(
        string name,
        ImmutableArray<BoundExpression> parameters,
        [NotNullWhen(true)] out FunctionSymbol? symbol)
    {
        if (TryGetClosest(name, parameters, out symbol))
        {
            return true;
        }

        if (_functions.TryGetValue(name, out var functions))
        {
            // Just return the first best function
            var anyFunction = functions.FirstOrDefault();
            if (anyFunction != null)
            {
                symbol = anyFunction;
                return true;
            }
        }

        symbol = null;
        return false;
    }
}
