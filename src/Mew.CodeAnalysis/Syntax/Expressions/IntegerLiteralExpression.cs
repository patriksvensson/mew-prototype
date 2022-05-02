namespace Mew.CodeAnalysis;

public sealed class IntegerLiteralExpression : LiteralExpression<long>
{
    public IntegerLiteralExpression(SyntaxTree syntaxTree, SyntaxToken token, long literal)
        : base(syntaxTree, token, literal)
    {
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitIntegerLiteral(this, context);
    }
}
