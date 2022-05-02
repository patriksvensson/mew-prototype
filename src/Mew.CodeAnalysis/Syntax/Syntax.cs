namespace Mew.CodeAnalysis;

public interface ISyntax
{
    SyntaxTree SyntaxTree { get; }
    TextSpan Span { get; }
    bool IsValid { get; }

    void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context);
}

public abstract class Syntax : ISyntax, IPositionable
{
    public abstract TextSpan Span { get; }
    public abstract bool IsValid { get; }
    public SyntaxTree SyntaxTree { get; }

    protected Syntax(SyntaxTree syntaxTree)
    {
        SyntaxTree = syntaxTree ?? throw new ArgumentNullException(nameof(syntaxTree));
    }

    [DebuggerStepThrough]
    public abstract void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context);

    [DebuggerStepThrough]
    protected static void EnsureSyntaxType(Syntax? syntax, Type[] expected, [CallerArgumentExpression("syntax")] string? parameterName = null)
    {
        if (syntax != null)
        {
            var type = syntax.GetType();
            if (!expected.Any(expectedType => expectedType.IsAssignableFrom(type)))
            {
                var expectedTypeNames = string.Join(",", expected.Select(t => t.Name));
                throw new ArgumentException(
                    $"Parameter '{parameterName}' is of an unexpected type '{type.Name}'. Expected types: {expectedTypeNames}",
                    parameterName);
            }
        }
    }

    [DebuggerStepThrough]
    protected static void EnsureSyntaxType(IEnumerable<Syntax>? syntaxes, Type[] expected, [CallerArgumentExpression("syntaxes")] string? parameterName = null)
    {
        if (syntaxes != null)
        {
            foreach (var syntax in syntaxes)
            {
                var type = syntax.GetType();
                if (!expected.Any(expectedType => expectedType.IsAssignableFrom(type)))
                {
                    var expectedTypeNames = string.Join(",", expected.Select(t => t.Name));
                    throw new ArgumentException(
                        $"An item in parameter '{parameterName}' is of an unexpected type '{type.Name}'. Expected types: {expectedTypeNames}",
                        parameterName);
                }
            }
        }
    }

    [DebuggerStepThrough]
    protected void EnsureTokenType(SyntaxToken? token, SyntaxTokenKind expected, [CallerArgumentExpression("token")] string? parameterName = null)
    {
        if (token != null && token.Kind != expected)
        {
            throw new ArgumentException(
                $"Parameter '{parameterName}' must be of type '{expected}' but was '{token.Kind}'.",
                parameterName);
        }
    }

    [DebuggerStepThrough]
    protected void EnsureAnyTokenType(SyntaxToken? token, SyntaxTokenKind[] expected, [CallerArgumentExpression("token")] string? parameterName = null)
    {
        if (token != null && !expected.Any(expectedType => token.Kind == expectedType))
        {
            var expectedTypeNames = string.Join(",", expected.Select(t => t));
            throw new ArgumentException(
                $"Parameter '{parameterName}' is of an unexpected token kind '{token.Kind}'. Expected token kinds: {expectedTypeNames}",
                parameterName);
        }
    }
}
