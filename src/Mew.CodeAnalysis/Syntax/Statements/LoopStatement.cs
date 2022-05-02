namespace Mew.CodeAnalysis;

public sealed class LoopStatement : StatementSyntax
{
    public SyntaxToken Keyword { get; }
    public Syntax Body { get; }

    public override TextSpan Span => TextSpan.Between(Keyword, Body);
    public override bool IsValid => Keyword.IsValid && Body.IsValid;

    public LoopStatement(SyntaxTree syntaxTree, SyntaxToken keyword, Syntax body)
        : base(syntaxTree)
    {
        EnsureSyntaxType(body, new[] { typeof(RecoverySyntax), typeof(BlockStatement) });

        Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
        Body = body ?? throw new ArgumentNullException(nameof(body));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitLoop(this, context);
    }
}