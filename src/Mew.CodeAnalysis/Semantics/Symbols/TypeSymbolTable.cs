namespace Mew.CodeAnalysis.Semantics;

public sealed class TypeSymbolTable
{
    private readonly Dictionary<string, TypeSymbol> _types;

    public TypeSymbolTable()
    {
        _types = PrimitiveTypeSymbol.All.ToDictionary(
            type => type.Name,
            type => type,
            StringComparer.Ordinal);
    }

    public bool Declare(TypeSymbol symbol)
    {
        if (_types.ContainsKey(symbol.Name))
        {
            return false;
        }

        _types.Add(symbol.Name, symbol);
        return true;
    }

    public bool HasType(TypeSymbol symbol)
    {
        return _types.ContainsKey(symbol.Name);
    }

    public bool TryGet(
        string name,
        [NotNullWhen(true)] out TypeSymbol? symbol)
    {
        if (_types.TryGetValue(name, out symbol))
        {
            return true;
        }

        symbol = null;
        return false;
    }

    public ImmutableArray<TypeSymbol> GetDeclared()
    {
        return _types.Values.ToImmutableArray();
    }
}
