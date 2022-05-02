namespace Mew.CodeAnalysis.Emit;

public sealed class ILEmitter
{
    private readonly ILEmitterContext _context;
    private readonly SemanticModel _model;

    public ILEmitter(SemanticModel model, string name)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        _context = new ILEmitterContext(name, new[]
        {
            @"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.4\ref\net6.0\System.Runtime.dll",
        });
    }

    public void Emit(Stream stream)
    {
        // Function declarations
        foreach (var function in _model.Functions)
        {
            EmitFunctionDeclaration(function.Key);
        }

        // External function declarations
        foreach (var function in _model.ExternalFunctions)
        {
            EmitExternalFunctionDeclaration(function);
        }

        // Function bodies
        foreach (var (function, body) in _model.Functions)
        {
            var method = _context.GetMethodDefinition(function);
            EmitFunctionBody(method, body.Statements);
        }

        EmitMain();

        // Write the assembly to the stream
        _context.Assembly.Write(stream);
    }

    private void EmitMain()
    {
        var voidType = _context.GetTypeReference(PrimitiveTypeSymbol.Void);
        var main = new MethodDefinition("Main", MethodAttributes.Static | MethodAttributes.Public, voidType);

        _context.ProgramType.Methods.Add(main);
        _context.Assembly.EntryPoint = main;

        var emitter = EmitFunctionBody(main, _model.Statements);

        // TODO: Insert return in semantic model if missing
        emitter.Emit(OpCodes.Ret);
    }

    private ILProcessor EmitFunctionBody(MethodDefinition method, IEnumerable<BoundStatement> statements)
    {
        _context.EnterMethod(method);

        foreach (var statement in statements)
        {
            statement.Accept(ILEmitterVisitor.Shared, _context);
        }

        foreach (var (instructionIndex, targetLabel) in _context.GetFixups())
        {
            var targetInstructionIndex = _context.GetLabelInstructionIndex(targetLabel);
            var targetInstruction = _context.Emitter.Body.Instructions[targetInstructionIndex];
            var instructionToFixup = _context.Emitter.Body.Instructions[instructionIndex];
            instructionToFixup.Operand = targetInstruction;
        }

        return _context.Emitter;
    }

    private void EmitFunctionDeclaration(DeclaredFunctionSymbol function)
    {
        var returnType = _context.GetTypeReference(function.ReturnType);
        var method = new MethodDefinition(function.Name, MethodAttributes.Static | MethodAttributes.Private, returnType);

        foreach (var parameter in function.Parameters)
        {
            method.Parameters.Add(
                new ParameterDefinition(
                    parameter.Name,
                    ParameterAttributes.None,
                    _context.GetTypeReference(parameter.Type)));
        }

        _context.AddMethod(function, method);
    }

    private void EmitExternalFunctionDeclaration(ExternalFunctionSymbol function)
    {
        var filename = function.GetLibraryFilename();

        var module = _context.Assembly.GetModuleReference(filename);
        if (module == null)
        {
            module = new ModuleReference(filename);
            _context.Assembly.MainModule.ModuleReferences.Add(module);
        }

        var returnType = _context.GetTypeReference(function.ReturnType);
        var method = new MethodDefinition(
            function.Name,
            MethodAttributes.Static | MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.PInvokeImpl, returnType);

        foreach (var parameter in function.Parameters)
        {
            method.Parameters.Add(
                new ParameterDefinition(
                    parameter.Name,
                    ParameterAttributes.None,
                    _context.GetTypeReference(parameter.Type)));
        }

        method.PInvokeInfo = new PInvokeInfo(PInvokeAttributes.CallConvWinapi, function.Name, module);
        method.AddAttribute<DllImportAttribute>(_context.Assembly.MainModule, filename);

        _context.AddMethod(function, method);
    }
}
