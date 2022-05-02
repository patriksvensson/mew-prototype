namespace Mew.CodeAnalysis;

public sealed class BlockStatement : StatementSyntax
{
    public Syntax LeftBrace { get; }
    public List<Syntax> Statements { get; }
    public Syntax RightBrace { get; }

    public override TextSpan Span => TextSpan.Between(LeftBrace, RightBrace);
    public override bool IsValid => LeftBrace.IsValid && Statements.All(s => s.IsValid) && RightBrace.IsValid;

    public BlockStatement(SyntaxTree syntaxTree, Syntax leftBrace, List<Syntax> statements, Syntax rightBrace)
        : base(syntaxTree)
    {
        LeftBrace = leftBrace;
        Statements = statements;
        RightBrace = rightBrace;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitBlock(this, context);
    }
}
