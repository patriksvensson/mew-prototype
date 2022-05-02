namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundLabel
{
    public string Name { get; }

    public BoundLabel(string name)
    {
        Name = name;
    }
}