namespace Mew.Testing;

public sealed class ParserFixture
{
    public static SyntaxTree Parse(string source)
    {
        return SyntaxTree.ParseText(source);
    }
}
