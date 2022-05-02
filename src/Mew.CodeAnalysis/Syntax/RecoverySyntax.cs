namespace Mew.CodeAnalysis;

public sealed class RecoverySyntax : Syntax
{
    public List<SyntaxToken> Tokens { get; }
    public override TextSpan Span { get; }
    public override bool IsValid { get; } = false;

    public RecoverySyntax(SyntaxTree syntaxTree, TextSpan span, IEnumerable<SyntaxToken> elements)
        : base(syntaxTree)
    {
        Span = span;
        Tokens = elements.AsList();
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitRecovery(this, context);
    }
}
