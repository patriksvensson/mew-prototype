namespace Mew.CodeAnalysis;

public abstract class StatementSyntax : Syntax
{
    protected StatementSyntax(SyntaxTree syntaxTree)
        : base(syntaxTree)
    {
    }
}
