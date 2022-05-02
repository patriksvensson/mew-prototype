namespace Mew.CodeAnalysis.Interpreting;

internal sealed class InterpreterVisitor : BoundNodeVisitor<InterpreterContext, object?>
{
    public static InterpreterVisitor Shared { get; } = new InterpreterVisitor();

    public object? VisitStatements(ImmutableArray<BoundStatement> statements, InterpreterContext context)
    {
        static Dictionary<BoundLabel, int> BuildIndex(ImmutableArray<BoundStatement> nodes)
        {
            var lookup = new Dictionary<BoundLabel, int>();
            for (var index = 0; index < nodes.Length; index++)
            {
                if (nodes[index] is BoundLabelStatement label)
                {
                    lookup.Add(label.Label, index);
                }
            }

            return lookup;
        }

        var index = 0;
        var lookup = BuildIndex(statements);

        while (index < statements.Length)
        {
            context.Current = statements[index].Syntax;

            switch (statements[index])
            {
                case BoundLabelStatement:
                    index++;
                    break;
                case BoundGotoStatement gotoStatement:
                    index = lookup[gotoStatement.Label];
                    break;
                case BoundConditionalGotoStatement conditionGoto:
                    var condition = (bool)Visit(conditionGoto.Condition, context)!;
                    if (condition == conditionGoto.JumpIfTrue)
                    {
                        index = lookup[conditionGoto.Label];
                    }
                    else
                    {
                        index++;
                    }

                    break;
                case BoundReturnStatement returnStatement:
                    context.LastValue = returnStatement.Expression == null
                        ? null : Visit(returnStatement.Expression, context)!;
                    return context.LastValue;
                default:
                    Visit(statements[index], context);
                    index++;
                    break;
            }
        }

        return context.LastValue;
    }

    [DebuggerStepThrough]
    protected override object? Visit(BoundNode? node, InterpreterContext context)
    {
        if (node?.Syntax != null)
        {
            context.Current = node.Syntax;
        }

        return base.Visit(node, context);
    }

    public override object? VisitAssignment(BoundAssignmentExpression node, InterpreterContext context)
    {
        var right = Visit(node.Expression, context);

        if (context.Stack.Count > 0)
        {
            if (context.Stack.Peek().TryGetValue(node.Variable, out var local))
            {
                context.Stack.Peek()[node.Variable] = right;
                return right;
            }
        }

        if (context.Globals.TryGetValue(node.Variable, out var value))
        {
            context.Globals[node.Variable] = right;
            return right;
        }

        throw new InterpreterRuntimeException(
            node.Syntax,
            $"The variable '{node.Variable.Name}' has not been declared.");
    }

    public override object? VisitBinaryExpression(BoundBinaryExpression node, InterpreterContext context)
    {
        context.Current = node.Syntax;

        var left = Visit(node.Left, context);
        var right = Visit(node.Right, context);

        context.Current = node.Syntax;

        switch (node.Op.Operator)
        {
            case BinaryOperator.Add:
                if (node.Op.Type == PrimitiveTypeSymbol.String)
                {
                    return (left as string ?? string.Empty)
                        + (right as string ?? string.Empty);
                }

                return (long)left! + (long)right!;
            case BinaryOperator.Subtract:
                return (long)left! - (long)right!;
            case BinaryOperator.Multiply:
                return (long)left! * (long)right!;
            case BinaryOperator.Divide:
                return (long)left! / (long)right!;
            case BinaryOperator.Modolu:
                return (long)left! % (long)right!;
            case BinaryOperator.NotEqual:
                return !Equals(left, right);
            case BinaryOperator.Equal:
                return Equals(left, right);
            case BinaryOperator.GreaterThan:
                return (long)left! > (long)right!;
            case BinaryOperator.GreaterThanOrEqual:
                return (long)left! >= (long)right!;
            case BinaryOperator.LessThan:
                return (long)left! < (long)right!;
            case BinaryOperator.LessThanOrEqual:
                return (long)left! <= (long)right!;
            default:
                throw new InvalidOperationException("Unexpected binary operator");
        }
    }

    public override object? VisitBlockStatement(BoundBlockStatement node, InterpreterContext context)
    {
        context.Current = node.Syntax;
        return VisitStatements(node.Statements, context);
    }

    public override object? VisitBooleanLiteralExpression(BoundBooleanLiteralExpression node, InterpreterContext context)
    {
        context.Current = node.Syntax;
        return node.Value;
    }

    public override object? VisitExpressionStatement(BoundExpressionStatement node, InterpreterContext context)
    {
        context.Current = node.Syntax;
        return Visit(node.Expression, context);
    }

    public override object? VisitFunctionCallExpression(BoundFunctionCallExpression node, InterpreterContext context)
    {
        context.Current = node.Syntax;

        // External function?
        if (node.Function is ExternalFunctionSymbol symbol)
        {
            var arguments = node.Arguments.Select(a => Visit(a, context)).ToImmutableArray();
            return context.Invoker.Invoke(symbol, arguments);
        }
        else if (node.Function is DeclaredFunctionSymbol declaredSymbol)
        {
            var scope = new Dictionary<VariableSymbol, object?>();
            for (var i = 0; i < node.Arguments.Length; i++)
            {
                var parameter = node.Function.Parameters[i];
                var value = Visit(node.Arguments[i], context);
                scope.Add(parameter, value);
            }

            context.Stack.Push(scope);
            var function = context.Model.Functions[declaredSymbol];
            var result = VisitStatements(function.Statements, context);
            context.Stack.Pop();

            return result;
        }
        else
        {
            throw new InterpreterRuntimeException(node.Syntax, "Unknown function type");
        }
    }

    public override object? VisitIntegerLiteralExpression(BoundIntegerLiteralExpression node, InterpreterContext context)
    {
        context.Current = node.Syntax;
        return node.Value;
    }

    public override object? VisitLetStatement(BoundLetStatement node, InterpreterContext context)
    {
        context.Current = node.Syntax;

        var value = Visit(node.Initializer, context);

        if (context.Stack.Count > 0)
        {
            context.Stack.Peek()[node.Variable] = value;
        }
        else
        {
            context.Globals[node.Variable] = value;
        }

        return value;
    }

    public override object? VisitLogicalExpression(BoundLogicalExpression node, InterpreterContext context)
    {
        context.Current = node.Syntax;

        var left = Visit(node.Left, context);
        var right = Visit(node.Right, context);

        context.Current = node.Syntax;

        switch (node.Op.Operator)
        {
            case LogicalOperator.And:
                return (bool)left! && (bool)right!;
            case LogicalOperator.Or:
                return (bool)left! || (bool)right!;
            default:
                throw new InterpreterRuntimeException(node.Syntax, "Unknown logical operator");
        }
    }

    public override object? VisitStringLiteralExpression(BoundStringLiteralExpression node, InterpreterContext context)
    {
        context.Current = node.Syntax;
        return node.Value;
    }

    public override object? VisitUnaryExpression(BoundUnaryExpression node, InterpreterContext context)
    {
        context.Current = node.Syntax;

        var operand = Visit(node.Operand, context);

        context.Current = node.Syntax;

        switch (node.Op.Operator)
        {
            case UnaryOperator.Negate:
                return !(bool)operand!;
            case UnaryOperator.Negative:
                return -(long)operand!;
            default:
                throw new InterpreterRuntimeException(node.Syntax, "Unknown unary operator");
        }
    }

    public override object? VisitVariableExpression(BoundVariableExpression node, InterpreterContext context)
    {
        context.Current = node.Syntax;

        if (context.Stack.Count > 0)
        {
            if (context.Stack.Peek().TryGetValue(node.Symbol, out var local))
            {
                return local;
            }
        }

        if (context.Globals.TryGetValue(node.Symbol, out var value))
        {
            return value;
        }

        throw new InterpreterRuntimeException(
            node.Syntax,
            $"The variable '{node.Symbol.Name}' has not been declared.");
    }

    public override object? VisitConditionalGotoStatement(BoundConditionalGotoStatement node, InterpreterContext context)
    {
        throw new InterpreterRuntimeException(node.Syntax, "Interpreter error. Reached unreachable statement");
    }

    public override object? VisitConversionExpression(BoundConversionExpression node, InterpreterContext context)
    {
        throw new InterpreterRuntimeException(node.Syntax, "Interpreter error. Not implemented");
    }

    public override object? VisitErrorExpression(BoundErrorExpression node, InterpreterContext context)
    {
        throw new InterpreterRuntimeException(node.Syntax, "Encountered error expression");
    }

    public override object? VisitGotoStatement(BoundGotoStatement node, InterpreterContext context)
    {
        throw new InterpreterRuntimeException(node.Syntax, "Interpreter error. Reached unreachable statement");
    }

    public override object? VisitIfStatement(BoundIfStatement node, InterpreterContext context)
    {
        throw new InterpreterRuntimeException(node.Syntax, "Interpreter error. Reached unreachable statement");
    }

    public override object? VisitLabelStatement(BoundLabelStatement node, InterpreterContext context)
    {
        throw new InterpreterRuntimeException(node.Syntax, "Interpreter error. Reached unreachable statement");
    }

    public override object? VisitLoopStatement(BoundLoopStatement node, InterpreterContext context)
    {
        throw new InterpreterRuntimeException(node.Syntax, "Interpreter error. Reached unreachable statement");
    }

    public override object? VisitReturnStatement(BoundReturnStatement node, InterpreterContext context)
    {
        throw new InterpreterRuntimeException(node.Syntax, "Interpreter error. Reached unreachable statement");
    }

    public override object? VisitWhileStatement(BoundWhileStatement node, InterpreterContext context)
    {
        throw new InterpreterRuntimeException(node.Syntax, "Interpreter error. Reached unreachable statement");
    }
}
