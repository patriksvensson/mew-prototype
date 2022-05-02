using LLVMSharp;

namespace Mew.CodeAnalysis.Emit;

public sealed class LLVMEmitterVisitor : BoundNodeVisitor<LLVMEmitterVisitor.Context>
{
    public static LLVMEmitterVisitor Shared { get; } = new LLVMEmitterVisitor();

    public sealed class Context
    {
        public LLVMModuleRef Module { get; }
        public LLVMBuilderRef Builder { get; }
        public Stack<LLVMValueRef> Stack { get; }
        public Dictionary<string, LLVMValueRef> Values { get; }
        public Dictionary<string, LLVMValueRef> Functions { get; }

        public Context(LLVMModuleRef module, LLVMBuilderRef builder)
        {
            Module = module;
            Builder = builder;
            Stack = new Stack<LLVMValueRef>();
            Values = new Dictionary<string, LLVMValueRef>();
            Functions = new Dictionary<string, LLVMValueRef>();
        }
    }

    public override void VisitBlockStatement(BoundBlockStatement node, Context context)
    {
        foreach (var statement in node.Statements)
        {
            Visit(statement, context);
        }
    }

    public override void VisitBinaryExpression(BoundBinaryExpression node, Context context)
    {
        Visit(node.Left, context);
        Visit(node.Right, context);

        var r = context.Stack.Pop();
        var l = context.Stack.Pop();

        switch (node.Op.Operator)
        {
            case BinaryOperator.Add:
                context.Stack.Push(LLVM.BuildAdd(context.Builder, l, r, "addtmp"));
                break;
            case BinaryOperator.Subtract:
                context.Stack.Push(LLVM.BuildSub(context.Builder, l, r, "subtmp"));
                break;
            case BinaryOperator.Multiply:
                context.Stack.Push(LLVM.BuildMul(context.Builder, l, r, "multmp"));
                break;
            case BinaryOperator.Divide:
            case BinaryOperator.Modolu:
            case BinaryOperator.NotEqual:
            case BinaryOperator.Equal:
            case BinaryOperator.GreaterThan:
            case BinaryOperator.GreaterThanOrEqual:
            case BinaryOperator.LessThan:
            case BinaryOperator.LessThanOrEqual:
            default:
                throw new EmitException($"Binary operator '{node.Op.Operator}' not supported yet");
        }
    }

    public override void VisitFunctionCallExpression(BoundFunctionCallExpression node, Context context)
    {
        var calleeF = LLVM.GetNamedFunction(context.Module, node.Function.Name);
        if (calleeF.Pointer == IntPtr.Zero)
        {
            throw new EmitException($"Unknown function '{node.Function.Name}' referenced");
        }

        if (LLVM.CountParams(calleeF) != node.Arguments.Length)
        {
            throw new EmitException($"Function '{node.Function.Name}' expected {node.Arguments.Length} argument(s)");
        }

        var argumentCount = (uint)node.Arguments.Length;
        var argsV = new LLVMValueRef[Math.Max(argumentCount, 1)];
        for (var i = 0; i < argumentCount; ++i)
        {
            node.Arguments[i].Accept(this, context);
            argsV[i] = context.Stack.Pop();
        }

        context.Stack.Push(LLVM.BuildCall(context.Builder, calleeF, argsV, string.Empty));
    }

    public override void VisitReturnStatement(BoundReturnStatement node, Context context)
    {
        if (node.Expression == null)
        {
            LLVM.BuildRetVoid(context.Builder);
            return;
        }

        node.Expression.Accept(this, context);
        LLVM.BuildRet(context.Builder, context.Stack.Pop());
    }

    public override void VisitBooleanLiteralExpression(BoundBooleanLiteralExpression node, Context context)
    {
        context.Stack.Push(LLVM.ConstReal(LLVM.DoubleType(), node.Value ? 1 : 0));
    }

    public override void VisitIntegerLiteralExpression(BoundIntegerLiteralExpression node, Context context)
    {
        context.Stack.Push(LLVM.ConstInt(LLVM.Int64Type(), (ulong)node.Value, new LLVMBool(0)));
    }

    public override void VisitVariableExpression(BoundVariableExpression node, Context context)
    {
        if (context.Values.TryGetValue(node.Symbol.Name, out var value))
        {
            context.Stack.Push(value);
        }
        else
        {
            throw new EmitException($"Unknown variable '{node.Symbol.Name}'");
        }
    }

    public override void VisitAssignment(BoundAssignmentExpression node, Context context)
    {
        throw new EmitException("Assignment expressions not supported yet");
    }

    public override void VisitConditionalGotoStatement(BoundConditionalGotoStatement node, Context context)
    {
        throw new EmitException("Conditional goto statements not supported yet");
    }

    public override void VisitConversionExpression(BoundConversionExpression node, Context context)
    {
        throw new EmitException("Conversion expressions not supported yet");
    }

    public override void VisitGotoStatement(BoundGotoStatement node, Context context)
    {
        throw new EmitException("Goto statements not supported yet");
    }

    public override void VisitLabelStatement(BoundLabelStatement node, Context context)
    {
        throw new EmitException("Label statements not supported yet");
    }

    public override void VisitLetStatement(BoundLetStatement node, Context context)
    {
        throw new EmitException("Let statements not supported yet");
    }

    public override void VisitLogicalExpression(BoundLogicalExpression node, Context context)
    {
        throw new EmitException("Logical expressions not supported yet");
    }

    public override void VisitStringLiteralExpression(BoundStringLiteralExpression node, Context context)
    {
        throw new EmitException("String literal expressions not supported yet");
    }

    public override void VisitUnaryExpression(BoundUnaryExpression node, Context context)
    {
        throw new EmitException("Unary expressions not supported yet");
    }

    public override void VisitErrorExpression(BoundErrorExpression node, Context context)
    {
        throw new EmitException("Loop statements are not supported in LLVM generation");
    }

    public override void VisitIfStatement(BoundIfStatement node, Context context)
    {
        throw new EmitException("If statements are not supported in LLVM generation. Should have been lowered");
    }

    public override void VisitLoopStatement(BoundLoopStatement node, Context context)
    {
        throw new EmitException("Loop statements are not supported in LLVM generation. Should have been lowered");
    }

    public override void VisitWhileStatement(BoundWhileStatement node, Context context)
    {
        throw new EmitException("While statements are not supported in LLVM generation. Should have been lowered");
    }
}