namespace Mew.CodeAnalysis.Semantics;

public abstract class Symbol
{
    public string Name { get; }
    public abstract SymbolKind Kind { get; }

    protected Symbol(string name)
    {
        Name = name;
    }
}
