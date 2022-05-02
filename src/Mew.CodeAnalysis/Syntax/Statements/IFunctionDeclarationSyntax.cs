namespace Mew.CodeAnalysis;

public interface IFunctionDeclarationSyntax : ISyntax
{
    IdentifierExpression Name { get; }
    ImmutableArray<Syntax> Parameters { get; }
    Syntax? ReturnType { get; }
}
