namespace Mew.CodeAnalysis.Semantics;

public sealed class PrimitiveTypeSymbol : TypeSymbol
{
    public static PrimitiveTypeSymbol Boolean { get; } = new PrimitiveTypeSymbol("bool");
    public static PrimitiveTypeSymbol Integer { get; } = new PrimitiveTypeSymbol("int");
    public static PrimitiveTypeSymbol String { get; } = new PrimitiveTypeSymbol("string");
    public static PrimitiveTypeSymbol Void { get; } = new PrimitiveTypeSymbol("void");

    public static IEnumerable<TypeSymbol> All { get; } = new[] { Boolean, Integer, String, Void };

    private PrimitiveTypeSymbol(string name)
        : base(name)
    {
    }
}