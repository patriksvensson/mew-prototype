namespace Mew.CodeAnalysis.Emit;

internal sealed class ILEmitterVisitor : BoundNodeVisitor<ILEmitterContext>
{
    public static ILEmitterVisitor Shared { get; } = new ILEmitterVisitor();

    public override void VisitAssignment(BoundAssignmentExpression node, ILEmitterContext context)
    {
        var variableDefinition = context.GetVariableDefinition(node.Variable);
        node.Expression.Accept(this, context);

        context.Emitter.Emit(OpCodes.Dup);
        context.Emitter.Emit(OpCodes.Stloc, variableDefinition);
    }

    public override void VisitBinaryExpression(BoundBinaryExpression node, ILEmitterContext context)
    {
        if (node.Op.Operator == BinaryOperator.Add)
        {
            if (node.Left.Type == PrimitiveTypeSymbol.String &&
               node.Right.Type == PrimitiveTypeSymbol.String)
            {
                throw new EmitException("String concatination not supported yet");
            }
        }

        node.Left.Accept(this, context);
        node.Right.Accept(this, context);

        switch (node.Op.Operator)
        {
            case BinaryOperator.Add:
                context.Emitter.Emit(OpCodes.Add);
                break;
            case BinaryOperator.Subtract:
                context.Emitter.Emit(OpCodes.Sub);
                break;
            case BinaryOperator.Multiply:
                context.Emitter.Emit(OpCodes.Mul);
                break;
            case BinaryOperator.Divide:
                context.Emitter.Emit(OpCodes.Div);
                break;
            case BinaryOperator.Modolu:
                context.Emitter.Emit(OpCodes.Rem);
                break;
            case BinaryOperator.NotEqual:
                context.Emitter.Emit(OpCodes.Ceq);
                context.Emitter.Emit(OpCodes.Ldc_I4_0);
                context.Emitter.Emit(OpCodes.Ceq);
                break;
            case BinaryOperator.Equal:
                context.Emitter.Emit(OpCodes.Ceq);
                break;
            case BinaryOperator.GreaterThan:
                context.Emitter.Emit(OpCodes.Cgt);
                break;
            case BinaryOperator.GreaterThanOrEqual:
                context.Emitter.Emit(OpCodes.Clt);
                context.Emitter.Emit(OpCodes.Ldc_I4_0);
                context.Emitter.Emit(OpCodes.Ceq);
                break;
            case BinaryOperator.LessThan:
                context.Emitter.Emit(OpCodes.Clt);
                break;
            case BinaryOperator.LessThanOrEqual:
                context.Emitter.Emit(OpCodes.Cgt);
                context.Emitter.Emit(OpCodes.Ldc_I4_0);
                context.Emitter.Emit(OpCodes.Ceq);
                break;
            default:
                throw new EmitException($"Unknown binary operator '{node.Op.Operator}'");
        }
    }

    public override void VisitBlockStatement(BoundBlockStatement node, ILEmitterContext context)
    {
        foreach (var statement in node.Statements)
        {
            statement.Accept(this, context);
        }
    }

    public override void VisitBooleanLiteralExpression(BoundBooleanLiteralExpression node, ILEmitterContext context)
    {
        context.Emitter.Emit(node.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
    }

    public override void VisitConditionalGotoStatement(BoundConditionalGotoStatement node, ILEmitterContext context)
    {
        node.Condition.Accept(this, context);

        var opCode = node.JumpIfTrue ? OpCodes.Brtrue : OpCodes.Brfalse;
        context.AddFixup(node.Label, context.Emitter.Body.Instructions.Count);
        context.Emitter.Emit(opCode, Instruction.Create(OpCodes.Nop));
    }

    public override void VisitExpressionStatement(BoundExpressionStatement node, ILEmitterContext context)
    {
        node.Expression.Accept(this, context);

        if (node.Expression.Type != PrimitiveTypeSymbol.Void)
        {
            context.Emitter.Emit(OpCodes.Pop);
        }
    }

    public override void VisitFunctionCallExpression(BoundFunctionCallExpression node, ILEmitterContext context)
    {
        foreach (var arg in node.Arguments)
        {
            arg.Accept(this, context);
        }

        var method = context.GetMethodDefinition(node.Function);
        context.Emitter.Emit(OpCodes.Call, method);
    }

    public override void VisitGotoStatement(BoundGotoStatement node, ILEmitterContext context)
    {
        context.AddFixup(node.Label, context.Emitter.Body.Instructions.Count);
        context.Emitter.Emit(OpCodes.Br, Instruction.Create(OpCodes.Nop));
    }

    public override void VisitLabelStatement(BoundLabelStatement node, ILEmitterContext context)
    {
        context.AddLabel(node.Label, context.Emitter.Body.Instructions.Count);
    }

    public override void VisitLetStatement(BoundLetStatement node, ILEmitterContext context)
    {
        var variableTypeRef = context.GetTypeReference(node.Variable.Type);
        var variableDefinition = new VariableDefinition(variableTypeRef);

        context.AddVariable(node.Variable, variableDefinition);
        context.Emitter.Body.Variables.Add(variableDefinition);

        node.Initializer.Accept(this, context);
        context.Emitter.Emit(OpCodes.Stloc, variableDefinition);
    }

    public override void VisitLogicalExpression(BoundLogicalExpression node, ILEmitterContext context)
    {
        node.Left.Accept(this, context);
        node.Right.Accept(this, context);

        switch (node.Op.Operator)
        {
            case LogicalOperator.And:
                context.Emitter.Emit(OpCodes.And);
                break;
            case LogicalOperator.Or:
                context.Emitter.Emit(OpCodes.Or);
                break;
        }
    }

    public override void VisitReturnStatement(BoundReturnStatement node, ILEmitterContext context)
    {
        if (node.Expression != null)
        {
            node.Expression.Accept(this, context);
        }

        context.Emitter.Emit(OpCodes.Ret);
    }

    public override void VisitUnaryExpression(BoundUnaryExpression node, ILEmitterContext context)
    {
        node.Operand.Accept(this, context);

        if (node.Op.Operator == UnaryOperator.Negative)
        {
            context.Emitter.Emit(OpCodes.Neg);
        }
        else if (node.Op.Operator == UnaryOperator.Negate)
        {
            context.Emitter.Emit(OpCodes.Ldc_I4_0);
            context.Emitter.Emit(OpCodes.Ceq);
        }
    }

    public override void VisitVariableExpression(BoundVariableExpression node, ILEmitterContext context)
    {
        var variableDefinition = context.GetVariableDefinition(node.Symbol);
        context.Emitter.Emit(OpCodes.Ldloc, variableDefinition);
    }

    public override void VisitIntegerLiteralExpression(BoundIntegerLiteralExpression node, ILEmitterContext context)
    {
        context.Emitter.Emit(OpCodes.Ldc_I8, node.Value);
    }

    public override void VisitStringLiteralExpression(BoundStringLiteralExpression node, ILEmitterContext context)
    {
        context.Emitter.Emit(OpCodes.Ldstr, node.Value);
    }

    protected override void VisitSymbol(BoundNode node, Symbol symbol, ILEmitterContext context)
    {
        // Do nothing
    }

    public override void VisitConversionExpression(BoundConversionExpression node, ILEmitterContext context)
    {
        throw new EmitException("Conversion expressions are not supported in IL generation");
    }

    public override void VisitErrorExpression(BoundErrorExpression node, ILEmitterContext context)
    {
        throw new EmitException("Error expressions are not supported in IL generation");
    }

    public override void VisitIfStatement(BoundIfStatement node, ILEmitterContext context)
    {
        throw new EmitException("If statements are not supported in IL generation. Should have been lowered");
    }

    public override void VisitLoopStatement(BoundLoopStatement node, ILEmitterContext context)
    {
        throw new EmitException("Loop statements are not supported in IL generation. Should have been lowered");
    }

    public override void VisitWhileStatement(BoundWhileStatement node, ILEmitterContext context)
    {
        throw new EmitException("While statements are not supported in IL generation. Should have been lowered");
    }
}
