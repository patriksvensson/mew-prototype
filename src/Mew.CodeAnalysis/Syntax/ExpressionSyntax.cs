namespace Mew.CodeAnalysis;

public abstract class ExpressionSyntax : Syntax
{
    protected ExpressionSyntax(SyntaxTree syntaxTree)
        : base(syntaxTree)
    {
    }
}
