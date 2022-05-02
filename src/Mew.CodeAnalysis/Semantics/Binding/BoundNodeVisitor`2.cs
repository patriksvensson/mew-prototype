namespace Mew.CodeAnalysis.Semantics;

public abstract class BoundNodeVisitor<TContext, TResult> : IBoundNodeVisitor<TContext, TResult>
{
    [DebuggerStepThrough]
    protected virtual TResult Visit(BoundNode? node, TContext context)
    {
        if (node == null)
        {
            throw new InvalidOperationException("Node was null");
        }

        RuntimeHelpers.EnsureSufficientExecutionStack();
        return node.Accept(this, context);
    }

    public abstract TResult VisitAssignment(BoundAssignmentExpression node, TContext context);
    public abstract TResult VisitBinaryExpression(BoundBinaryExpression node, TContext context);
    public abstract TResult VisitBlockStatement(BoundBlockStatement node, TContext context);
    public abstract TResult VisitBooleanLiteralExpression(BoundBooleanLiteralExpression node, TContext context);
    public abstract TResult VisitConversionExpression(BoundConversionExpression node, TContext context);
    public abstract TResult VisitConditionalGotoStatement(BoundConditionalGotoStatement node, TContext context);
    public abstract TResult VisitErrorExpression(BoundErrorExpression node, TContext context);
    public abstract TResult VisitExpressionStatement(BoundExpressionStatement node, TContext context);
    public abstract TResult VisitFunctionCallExpression(BoundFunctionCallExpression node, TContext context);
    public abstract TResult VisitGotoStatement(BoundGotoStatement node, TContext context);
    public abstract TResult VisitIfStatement(BoundIfStatement node, TContext context);
    public abstract TResult VisitIntegerLiteralExpression(BoundIntegerLiteralExpression node, TContext context);
    public abstract TResult VisitLabelStatement(BoundLabelStatement node, TContext context);
    public abstract TResult VisitLetStatement(BoundLetStatement node, TContext context);
    public abstract TResult VisitLogicalExpression(BoundLogicalExpression node, TContext context);
    public abstract TResult VisitLoopStatement(BoundLoopStatement node, TContext context);
    public abstract TResult VisitReturnStatement(BoundReturnStatement node, TContext context);
    public abstract TResult VisitStringLiteralExpression(BoundStringLiteralExpression node, TContext context);
    public abstract TResult VisitUnaryExpression(BoundUnaryExpression node, TContext context);
    public abstract TResult VisitVariableExpression(BoundVariableExpression node, TContext context);
    public abstract TResult VisitWhileStatement(BoundWhileStatement node, TContext context);
}