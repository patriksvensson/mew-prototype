namespace Mew.CodeAnalysis.Emit;

internal sealed class LLVMFunctionDefinition
{
    public string Name { get; }
    public IReadOnlyList<ParameterSymbol> Parameters { get; }
    public TypeSymbol ReturnType { get; }
    public IReadOnlyList<BoundStatement>? Statements { get; }

    public LLVMFunctionDefinition(
        string name,
        IEnumerable<ParameterSymbol> parameters,
        TypeSymbol returnType,
        IEnumerable<BoundStatement>? statements)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Parameters = new List<ParameterSymbol>(parameters ?? Enumerable.Empty<ParameterSymbol>());
        ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
        Statements = statements == null
            ? null : new List<BoundStatement>(statements);
    }
}
