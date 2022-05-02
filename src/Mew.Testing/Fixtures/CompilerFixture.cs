using Mew.CodeAnalysis.Semantics;

namespace Mew.Testing;

public static class CompilerFixture
{
    public static Compilation GetCompilation(string source)
    {
        var syntaxTree = SyntaxTree.ParseText(source);
        return Compilation.Create("test")
            .AddSyntaxTree(syntaxTree);
    }

    public static SemanticModel GetSemanticModel(string source)
    {
        return GetCompilation(source)
            .GetSemanticModel();
    }
}
