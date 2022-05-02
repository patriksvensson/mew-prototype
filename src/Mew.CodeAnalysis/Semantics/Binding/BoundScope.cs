namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundScope
{
    private readonly Dictionary<string, Symbol> _symbols;

    public BoundScope? Parent { get; }
    public TypeSymbolTable Types { get; }
    public FunctionSymbolTable Functions { get; }

    public BoundScope(BoundScope? parent)
    {
        _symbols = new Dictionary<string, Symbol>();

        Parent = parent;
        Types = parent?.Types ?? new TypeSymbolTable();
        Functions = parent?.Functions ?? new FunctionSymbolTable();
    }

    public bool TryDeclareVariable(ParameterSymbol parameter)
    {
        return TryDeclareSymbol(parameter);
    }

    public bool TryDeclareVariable(VariableSymbol variable)
    {
        return TryDeclareSymbol(variable);
    }

    public bool TryLookupSymbol(string name, [NotNullWhen(true)] out Symbol? symbol)
    {
        if (_symbols.TryGetValue(name, out symbol))
        {
            return true;
        }

        return Parent?.TryLookupSymbol(name, out symbol) ?? false;
    }

    public ImmutableArray<VariableSymbol> GetDeclaredVariables()
    {
        return GetDeclaredSymbols<VariableSymbol>();
    }

    private bool TryDeclareSymbol<TSymbol>(TSymbol symbol)
        where TSymbol : Symbol
    {
        if (_symbols.ContainsKey(symbol.Name))
        {
            return false;
        }

        _symbols.Add(symbol.Name, symbol);
        return true;
    }

    private ImmutableArray<TSymbol> GetDeclaredSymbols<TSymbol>()
        where TSymbol : Symbol
    {
        if (_symbols.Count == 0)
        {
            return ImmutableArray<TSymbol>.Empty;
        }

        return _symbols.Values.OfType<TSymbol>().ToImmutableArray();
    }
}