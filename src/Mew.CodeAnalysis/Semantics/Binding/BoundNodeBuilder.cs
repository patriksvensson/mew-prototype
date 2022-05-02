namespace Mew.CodeAnalysis.Semantics;

internal static class BoundNodeBuilder
{
    public static BoundBlockStatement Block(Syntax syntax, BoundStatement[] statements)
    {
        return new BoundBlockStatement(syntax, statements.ToImmutableArray());
    }

    public static BoundGotoStatement Goto(Syntax syntax, BoundLabel label)
    {
        return new BoundGotoStatement(syntax, label);
    }

    public static BoundConditionalGotoStatement GotoTrue(Syntax syntax, BoundLabel label, BoundExpression condition)
    {
        return new BoundConditionalGotoStatement(syntax, label, condition, jumpIfTrue: true);
    }

    public static BoundConditionalGotoStatement GotoFalse(Syntax syntax, BoundLabel label, BoundExpression condition)
    {
        return new BoundConditionalGotoStatement(syntax, label, condition, jumpIfTrue: false);
    }

    public static BoundLabelStatement Label(Syntax syntax, BoundLabel label)
    {
        return new BoundLabelStatement(syntax, label);
    }
}
