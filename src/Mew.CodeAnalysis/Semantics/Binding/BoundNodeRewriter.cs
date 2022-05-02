namespace Mew.CodeAnalysis.Semantics;

public abstract class BoundNodeRewriter<TContext> : IBoundNodeVisitor<TContext, BoundNode>
{
    protected BoundExpression RewriteExpression(BoundNode node, TContext context)
    {
        var result = node.Accept(this, context);
        if (result is not BoundExpression expression)
        {
            throw new InvalidOperationException("Rewritten node was not an expression");
        }

        return expression;
    }

    protected BoundStatement RewriteStatement(BoundNode node, TContext context)
    {
        var result = node.Accept(this, context);
        if (result is not BoundStatement statement)
        {
            throw new InvalidOperationException("Rewritten node was not a statement");
        }

        return statement;
    }

    public virtual BoundNode VisitAssignment(BoundAssignmentExpression node, TContext context)
    {
        var expression = RewriteExpression(node.Expression, context);
        if (expression == node.Expression)
        {
            return node;
        }

        return new BoundAssignmentExpression(node.Syntax, node.Variable, expression);
    }

    public virtual BoundNode VisitBinaryExpression(BoundBinaryExpression node, TContext context)
    {
        var left = RewriteExpression(node.Left, context);
        var right = RewriteExpression(node.Right, context);

        if (left == node.Left && right == node.Right)
        {
            return node;
        }

        return new BoundBinaryExpression(node.Syntax, left, node.Op, right);
    }

    public virtual BoundNode VisitBlockStatement(BoundBlockStatement node, TContext context)
    {
        ImmutableArray<BoundStatement>.Builder? builder = null;

        for (var i = 0; i < node.Statements.Length; i++)
        {
            var oldStatement = node.Statements[i];
            var newStatement = RewriteStatement(oldStatement, context);
            if (newStatement != oldStatement)
            {
                if (builder == null)
                {
                    builder = ImmutableArray.CreateBuilder<BoundStatement>(node.Statements.Length);

                    for (var j = 0; j < i; j++)
                    {
                        builder.Add(node.Statements[j]);
                    }
                }
            }

            builder?.Add(newStatement);
        }

        if (builder == null)
        {
            return node;
        }

        return new BoundBlockStatement(node.Syntax, builder.MoveToImmutable());
    }

    public virtual BoundNode VisitBooleanLiteralExpression(BoundBooleanLiteralExpression node, TContext context)
    {
        return node;
    }

    public virtual BoundNode VisitConditionalGotoStatement(BoundConditionalGotoStatement node, TContext context)
    {
        var condition = RewriteExpression(node.Condition, context);
        if (condition == node.Condition)
        {
            return node;
        }

        return new BoundConditionalGotoStatement(node.Syntax, node.Label, condition, node.JumpIfTrue);
    }

    public virtual BoundNode VisitConversionExpression(BoundConversionExpression node, TContext context)
    {
        var expression = RewriteExpression(node.Expression, context);
        if (expression == node.Expression)
        {
            return node;
        }

        return new BoundConversionExpression(node.Syntax, node.Type, expression);
    }

    public virtual BoundNode VisitErrorExpression(BoundErrorExpression node, TContext context)
    {
        return node;
    }

    public virtual BoundNode VisitExpressionStatement(BoundExpressionStatement node, TContext context)
    {
        var expression = RewriteExpression(node.Expression, context);
        if (expression == node.Expression)
        {
            return node;
        }

        return new BoundExpressionStatement(node.Syntax, expression);
    }

    public virtual BoundNode VisitFunctionCallExpression(BoundFunctionCallExpression node, TContext context)
    {
        ImmutableArray<BoundExpression>.Builder? builder = null;

        for (var i = 0; i < node.Arguments.Length; i++)
        {
            var oldArgument = node.Arguments[i];
            var newArgument = RewriteExpression(oldArgument, context);

            if (newArgument != oldArgument)
            {
                if (builder == null)
                {
                    builder = ImmutableArray.CreateBuilder<BoundExpression>(node.Arguments.Length);
                    for (var j = 0; j < i; j++)
                    {
                        builder.Add(node.Arguments[j]);
                    }
                }

                builder?.Add(newArgument);
            }
        }

        if (builder == null)
        {
            return node;
        }

        return new BoundFunctionCallExpression(node.Syntax, node.Function, builder.ToImmutable());
    }

    public virtual BoundNode VisitGotoStatement(BoundGotoStatement node, TContext context)
    {
        return node;
    }

    public virtual BoundNode VisitIfStatement(BoundIfStatement node, TContext context)
    {
        var condition = RewriteExpression(node.Condition, context);
        var thenBranch = RewriteStatement(node.ThenBranch, context);
        var elseBranch = node.ElseBranch != null ? RewriteStatement(node.ThenBranch, context) : null;

        if (condition == node.Condition
           && thenBranch == node.ThenBranch
           && elseBranch == node.ElseBranch)
        {
            return node;
        }

        return new BoundIfStatement(node.Syntax, condition, thenBranch, elseBranch);
    }

    public virtual BoundNode VisitIntegerLiteralExpression(BoundIntegerLiteralExpression node, TContext context)
    {
        return node;
    }

    public virtual BoundNode VisitLabelStatement(BoundLabelStatement node, TContext context)
    {
        return node;
    }

    public virtual BoundNode VisitLetStatement(BoundLetStatement node, TContext context)
    {
        var initializer = RewriteExpression(node.Initializer, context);
        if (initializer == node.Initializer)
        {
            return node;
        }

        return new BoundLetStatement(node.Syntax, node.Variable, initializer);
    }

    public virtual BoundNode VisitLogicalExpression(BoundLogicalExpression node, TContext context)
    {
        var left = RewriteExpression(node.Left, context);
        var right = RewriteExpression(node.Right, context);

        if (left == node.Left && right == node.Right)
        {
            return node;
        }

        return new BoundLogicalExpression(node.Syntax, left, node.Op, right);
    }

    public virtual BoundNode VisitLoopStatement(BoundLoopStatement node, TContext context)
    {
        var body = RewriteStatement(node.Body, context);
        if (body == node.Body)
        {
            return node;
        }

        return new BoundLoopStatement(node.Syntax, body, node.BreakLabel, node.ContinueLabel);
    }

    public virtual BoundNode VisitReturnStatement(BoundReturnStatement node, TContext context)
    {
        var expression = node.Expression != null ? RewriteExpression(node.Expression, context) : null;
        if (expression == node.Expression)
        {
            return node;
        }

        return new BoundReturnStatement(node.Syntax, expression);
    }

    public virtual BoundNode VisitStringLiteralExpression(BoundStringLiteralExpression node, TContext context)
    {
        return node;
    }

    public virtual BoundNode VisitUnaryExpression(BoundUnaryExpression node, TContext context)
    {
        var operand = RewriteExpression(node.Operand, context);
        if (operand == node.Operand)
        {
            return node;
        }

        return new BoundUnaryExpression(node.Syntax, operand, node.Op);
    }

    public virtual BoundNode VisitVariableExpression(BoundVariableExpression node, TContext context)
    {
        return node;
    }

    public virtual BoundNode VisitWhileStatement(BoundWhileStatement node, TContext context)
    {
        var condition = RewriteExpression(node.Condition, context);
        var body = RewriteStatement(node.Body, context);

        if (condition == node.Condition && body == node.Body)
        {
            return node;
        }

        return new BoundWhileStatement(node.Syntax, condition, body, node.BreakLabel, node.ContinueLabel);
    }
}
