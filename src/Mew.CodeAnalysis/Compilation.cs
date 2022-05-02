namespace Mew.CodeAnalysis;

public sealed class Compilation
{
    private readonly ImmutableArray<SyntaxTree> _syntaxTrees;
    private SemanticModel? _semanticModel;

    public string Name { get; }

    private Compilation(string name, ImmutableArray<SyntaxTree> syntaxTrees)
    {
        Name = name;
        _syntaxTrees = syntaxTrees;
    }

    public static Compilation Create(string name)
    {
        return new Compilation(name, ImmutableArray<SyntaxTree>.Empty);
    }

    public Compilation AddSyntaxTree(SyntaxTree tree)
    {
        var builder = _syntaxTrees.ToBuilder();
        builder.Add(tree);
        return new Compilation(Name, builder.ToImmutable());
    }

    public Diagnostics GetDiagnostics()
    {
        return GetSemanticModel().Diagnostics;
    }

    public SemanticModel GetSemanticModel()
    {
        return _semanticModel ??= SemanticModel.Create(
            Name,
            _syntaxTrees);
    }
}
