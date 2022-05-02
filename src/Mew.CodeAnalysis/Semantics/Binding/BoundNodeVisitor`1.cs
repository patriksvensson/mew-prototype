namespace Mew.CodeAnalysis.Semantics;

public abstract class BoundNodeVisitor<TContext> : IBoundNodeVisitor<TContext>
{
    protected virtual void Visit(BoundNode? node, TContext context)
    {
        if (node == null)
        {
            return;
        }

        RuntimeHelpers.EnsureSufficientExecutionStack();
        node.Accept(this, context);
    }

    protected virtual void VisitSymbol(BoundNode node, Symbol symbol, TContext context)
    {
    }

    public virtual void VisitAssignment(BoundAssignmentExpression node, TContext context)
    {
        VisitSymbol(node, node.Variable, context);
        Visit(node.Expression, context);
    }

    public virtual void VisitBinaryExpression(BoundBinaryExpression node, TContext context)
    {
        VisitSymbol(node, node.Type, context);
        Visit(node.Left, context);
        Visit(node.Right, context);
    }

    public virtual void VisitBlockStatement(BoundBlockStatement node, TContext context)
    {
        foreach (var statement in node.Statements)
        {
            Visit(statement, context);
        }
    }

    public virtual void VisitBooleanLiteralExpression(BoundBooleanLiteralExpression node, TContext context)
    {
        VisitSymbol(node, node.Type, context);
    }

    public virtual void VisitConversionExpression(BoundConversionExpression node, TContext context)
    {
        VisitSymbol(node, node.Type, context);
        Visit(node.Expression, context);
    }

    public virtual void VisitConditionalGotoStatement(BoundConditionalGotoStatement node, TContext context)
    {
        Visit(node.Condition, context);
    }

    public virtual void VisitErrorExpression(BoundErrorExpression node, TContext context)
    {
    }

    public virtual void VisitExpressionStatement(BoundExpressionStatement node, TContext context)
    {
        Visit(node.Expression, context);
    }

    public virtual void VisitFunctionCallExpression(BoundFunctionCallExpression node, TContext context)
    {
        VisitSymbol(node, node.Function, context);

        foreach (var argument in node.Arguments)
        {
            VisitSymbol(argument, argument.Type, context);
            Visit(argument, context);
        }
    }

    public virtual void VisitGotoStatement(BoundGotoStatement node, TContext context)
    {
    }

    public virtual void VisitIfStatement(BoundIfStatement node, TContext context)
    {
        Visit(node.Condition, context);
        Visit(node.ThenBranch, context);
        Visit(node.ElseBranch, context);
    }

    public virtual void VisitIntegerLiteralExpression(BoundIntegerLiteralExpression node, TContext context)
    {
        VisitSymbol(node, node.Type, context);
    }

    public virtual void VisitLabelStatement(BoundLabelStatement node, TContext context)
    {
    }

    public virtual void VisitLetStatement(BoundLetStatement node, TContext context)
    {
        VisitSymbol(node, node.Variable, context);
        Visit(node.Initializer, context);
    }

    public virtual void VisitLogicalExpression(BoundLogicalExpression node, TContext context)
    {
        VisitSymbol(node, node.Type, context);
        Visit(node.Left, context);
        Visit(node.Right, context);
    }

    public virtual void VisitLoopStatement(BoundLoopStatement node, TContext context)
    {
        Visit(node.Body, context);
    }

    public virtual void VisitReturnStatement(BoundReturnStatement node, TContext context)
    {
        Visit(node.Expression, context);
    }

    public virtual void VisitStringLiteralExpression(BoundStringLiteralExpression node, TContext context)
    {
        VisitSymbol(node, node.Type, context);
    }

    public virtual void VisitUnaryExpression(BoundUnaryExpression node, TContext context)
    {
        VisitSymbol(node, node.Type, context);
        Visit(node.Operand, context);
    }

    public virtual void VisitVariableExpression(BoundVariableExpression node, TContext context)
    {
        VisitSymbol(node, node.Symbol, context);
    }

    public virtual void VisitWhileStatement(BoundWhileStatement node, TContext context)
    {
        Visit(node.Condition, context);
        Visit(node.Body, context);
    }
}
