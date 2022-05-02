namespace Mew.CodeAnalysis.Emit;

internal sealed class ILEmitterContext
{
    private readonly List<AssemblyDefinition> _referenceAssemblies;
    private readonly Dictionary<TypeSymbol, TypeReference> _knownTypes;
    private readonly Dictionary<FunctionSymbol, MethodDefinition> _methods;

    private readonly Dictionary<BoundLabel, int> _labels;
    private readonly Dictionary<VariableSymbol, VariableDefinition> _locals;
    private readonly List<(int InstructionIndex, BoundLabel TargetLabel)> _fixups;

    private ILProcessor? _iLProcessor;

    public ILProcessor Emitter
    {
        get
        {
            if (_iLProcessor == null)
            {
                throw new EmitException("Could not get IL processor for current context");
            }

            return _iLProcessor;
        }
    }

    public AssemblyDefinition Assembly { get; }
    public TypeDefinition ProgramType { get; }

    public ILEmitterContext(string name, IEnumerable<string> references)
    {
        _methods = new Dictionary<FunctionSymbol, MethodDefinition>();
        _locals = new Dictionary<VariableSymbol, VariableDefinition>();
        _labels = new Dictionary<BoundLabel, int>();
        _fixups = new List<(int InstructionIndex, BoundLabel Target)>();

        // Create the assembly
        var assemblyName = new AssemblyNameDefinition(name, new Version(1, 0));
        Assembly = AssemblyDefinition.CreateAssembly(assemblyName, name, ModuleKind.Console);

        // Get all reference assemblies and known types
        _referenceAssemblies = LoadReferenceAssemblies(references);
        _knownTypes = GetKnownTypes();

        // Create the type for our program
        var obj = ResolveType(typeof(object));
        ProgramType = new TypeDefinition(string.Empty, "Program", TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Public, obj);
        Assembly.MainModule.Types.Add(ProgramType);
    }

    public void EnterMethod(MethodDefinition method)
    {
        _iLProcessor = method.Body.GetILProcessor();
        _locals.Clear();
        _labels.Clear();
        _fixups.Clear();
    }

    public int GetLabelInstructionIndex(BoundLabel label)
    {
        if (!_labels.TryGetValue(label, out var index))
        {
            throw new EmitException($"Could not resolve instruction index for label '{label.Name}'");
        }

        return index;
    }

    public MethodDefinition GetMethodDefinition(FunctionSymbol function)
    {
        if (!_methods.TryGetValue(function, out var methodDefinition))
        {
            throw new EmitException($"Could not resolve function '{function.Name}'");
        }

        return methodDefinition;
    }

    public TypeReference GetTypeReference(TypeSymbol type)
    {
        if (!_knownTypes.TryGetValue(type, out var typeReference))
        {
            throw new EmitException($"Could not resolve type '{type.Name}'");
        }

        return typeReference;
    }

    public VariableDefinition GetVariableDefinition(VariableSymbol symbol)
    {
        if (!_locals.TryGetValue(symbol, out var variableDefinition))
        {
            throw new EmitException($"Could not resolve variable definition for '{symbol.Name}'");
        }

        return variableDefinition;
    }

    public IEnumerable<(int InstructionIndex, BoundLabel TargetLabel)> GetFixups()
    {
        return _fixups;
    }

    public void AddMethod(FunctionSymbol function, MethodDefinition method)
    {
        ProgramType.Methods.Add(method);
        _methods.Add(function, method);
    }

    public void AddVariable(VariableSymbol symbol, VariableDefinition variable)
    {
        _locals[symbol] = variable;
    }

    public void AddFixup(BoundLabel label, int instructionIndex)
    {
        _fixups.Add((instructionIndex, label));
    }

    public void AddLabel(BoundLabel label, int instructionIndex)
    {
        _labels[label] = instructionIndex;
    }

    public TypeReference? ResolveType(Type type)
    {
        var references = _referenceAssemblies.SelectMany(a => a.Modules)
            .SelectMany(m => m.Types)
            .Where(t => t.FullName == type.FullName)
            .ToArray();

        if (references.Length == 1)
        {
            return Assembly.MainModule.ImportReference(references[0]);
        }

        // TODO: Report diagnostics
        return null;
    }

    private static List<AssemblyDefinition> LoadReferenceAssemblies(IEnumerable<string> references)
    {
        var assemblies = new List<AssemblyDefinition>();
        foreach (var path in references)
        {
            try
            {
                var assembly = AssemblyDefinition.ReadAssembly(path);
                assemblies.Add(assembly);
            }
            catch (BadImageFormatException)
            {
                // TODO: Report diagnostics
            }
        }

        return assemblies;
    }

    private Dictionary<TypeSymbol, TypeReference> GetKnownTypes()
    {
        var types = new (TypeSymbol Symbol, Type Type)[]
        {
            (PrimitiveTypeSymbol.String, typeof(string)),
            (PrimitiveTypeSymbol.Integer, typeof(long)),
            (PrimitiveTypeSymbol.Boolean, typeof(bool)),
            (PrimitiveTypeSymbol.Void, typeof(void)),
        };

        var resolvedTypes = types.Select(type => (type.Symbol, Definition: ResolveType(type.Type)));

        var result = new Dictionary<TypeSymbol, TypeReference>();
        foreach (var type in resolvedTypes)
        {
            if (type.Definition != null)
            {
                result[type.Symbol] = Assembly.MainModule.ImportReference(type.Definition);
            }
        }

        return result;
    }
}
