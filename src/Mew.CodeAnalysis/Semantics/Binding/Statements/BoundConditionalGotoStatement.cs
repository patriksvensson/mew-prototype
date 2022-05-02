namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundConditionalGotoStatement : BoundStatement
{
    public BoundLabel Label { get; }
    public BoundExpression Condition { get; }
    public bool JumpIfTrue { get; }

    public BoundConditionalGotoStatement(Syntax syntax, BoundLabel label, BoundExpression condition, bool jumpIfTrue)
        : base(syntax)
    {
        Label = label ?? throw new ArgumentNullException(nameof(label));
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        JumpIfTrue = jumpIfTrue;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitConditionalGotoStatement(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitConditionalGotoStatement(this, context);
    }
}
