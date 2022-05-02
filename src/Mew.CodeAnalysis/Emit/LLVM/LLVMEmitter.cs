using LLVMSharp;

namespace Mew.CodeAnalysis.Emit;

public sealed class LLVMEmitter
{
    private readonly Action<string> _logCallback;

    public LLVMEmitter(Action<string> logCallback)
    {
        _logCallback = logCallback;
    }

    public void Emit(SemanticModel semanticModel, string filename, bool dump)
    {
        var context = new LLVMEmitterVisitor.Context(
            LLVM.ModuleCreateWithName(semanticModel.Name),
            LLVM.CreateBuilder());

        // Emit functions
        foreach (var (symbol, syntax) in semanticModel.Functions)
        {
            EmitFunction(context, symbol, syntax);
        }

        // Emit external functions
        foreach (var symbol in semanticModel.ExternalFunctions)
        {
            EmitFunction(context, symbol, null);
        }

        // Emit the main method
        EmitFunction(
            context,
            new LLVMFunctionDefinition(
                "main",
                Enumerable.Empty<ParameterSymbol>(),
                PrimitiveTypeSymbol.Integer,
                semanticModel.Statements),
            emitReturn: false);

        LLVM.VerifyModule(context.Module, LLVMVerifierFailureAction.LLVMPrintMessageAction, out _);

        if (dump)
        {
            _logCallback.Invoke("LLVM IR:");
            LLVM.DumpModule(context.Module);
        }

        _logCallback.Invoke($"Emitting: [yellow]{filename}[/]");
        LLVM.WriteBitcodeToFile(context.Module, filename);
    }

    private void EmitFunction(
        LLVMEmitterVisitor.Context context,
        FunctionSymbol function,
        BoundBlockStatement? node)
    {
        EmitFunction(
            context,
            new LLVMFunctionDefinition(
                function.Name,
                function.Parameters,
                function.ReturnType,
                node?.Statements));
    }

    private void EmitFunction(
        LLVMEmitterVisitor.Context context,
        LLVMFunctionDefinition definition,
        bool emitReturn = false)
    {
        context.Stack.Clear();
        context.Values.Clear();

        var arguments = new LLVMTypeRef[definition.Parameters.Count];

        var function = LLVM.GetNamedFunction(context.Module, definition.Name);
        if (function.Pointer != IntPtr.Zero)
        {
            throw new EmitException($"Function '{definition.Name}' has already been defined");
        }
        else
        {
            for (var i = 0; i < definition.Parameters.Count; i++)
            {
                arguments[i] = GetTypeRef(definition.Parameters[i].Type);
            }

            function = LLVM.AddFunction(context.Module, definition.Name, LLVM.FunctionType(GetTypeRef(definition.ReturnType), arguments, new LLVMBool(0)));
            LLVM.SetLinkage(function, LLVMLinkage.LLVMExternalLinkage);
        }

        for (var i = 0; i < definition.Parameters.Count; i++)
        {
            var parameter = LLVM.GetParam(function, (uint)i);
            LLVM.SetValueName(parameter, definition.Parameters[i].Name);
            context.Values[definition.Parameters[i].Name] = parameter;
        }

        if (definition.Statements != null)
        {
            LLVM.PositionBuilderAtEnd(context.Builder, LLVM.AppendBasicBlock(function, "entry"));

            foreach (var statement in definition.Statements)
            {
                statement.Accept(LLVMEmitterVisitor.Shared, context);
            }

            if (emitReturn)
            {
                LLVM.BuildRet(context.Builder, LLVM.ConstInt(LLVM.Int64Type(), 32, new LLVMBool(0)));
            }
        }

        LLVM.VerifyFunction(function, LLVMVerifierFailureAction.LLVMPrintMessageAction);
    }

    private static LLVMTypeRef GetTypeRef(TypeSymbol type)
    {
        if (type == PrimitiveTypeSymbol.Integer)
        {
            return LLVM.Int64Type();
        }

        if (type == PrimitiveTypeSymbol.Boolean)
        {
            return LLVM.DoubleType();
        }

        if (type == PrimitiveTypeSymbol.Void)
        {
            return LLVM.VoidType();
        }

        throw new EmitException($"Type '{type.Name}' is not supported yet");
    }
}
