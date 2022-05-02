namespace Mew.CodeAnalysis;

public sealed class SyntaxTree
{
    public string Path { get; }
    public string Source { get; }
    public CompilationUnit Root { get; }
    public Diagnostics Diagnostics { get; }

    // A bit of "the chicken and the egg" here. We need the syntax tree to parse something
    // but the parser need the syntax tree to it can assign it to created nodes.
    private SyntaxTree(string path, string source, Func<SyntaxTree, (CompilationUnit Root, Diagnostics Diagnostics)> func)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
        Source = source ?? throw new ArgumentNullException(nameof(source));

        // Evaluate the parser
        var (root, diagnostics) = func(this);

        Root = root;
        Diagnostics = diagnostics;
    }

    public static SyntaxTree ParseText(string source)
    {
        return Parse(string.Empty, source);
    }

    public static SyntaxTree Parse(string path, string source)
    {
        return new SyntaxTree(path, source, tree =>
        {
            var parser = new Parser();
            return parser.Parse(tree);
        });
    }
}
