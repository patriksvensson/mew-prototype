namespace Mew.CodeAnalysis;

internal sealed class SyntaxTokenReader
{
    public IReadOnlyList<SyntaxToken> Tokens { get; }
    public int Position { get; set; }

    public SyntaxToken Current => Tokens[Position];

    public SyntaxTokenReader(IReadOnlyList<SyntaxToken> tokens)
    {
        Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        Position = 0;
    }

    public SyntaxToken Advance()
    {
        if (!IsAtEnd())
        {
            Position++;
        }

        return Previous();
    }

    public bool IsAtEnd()
    {
        return Position >= Tokens.Count || Peek().Kind == SyntaxTokenKind.Eof;
    }

    public SyntaxToken Peek()
    {
        return Tokens[Position];
    }

    public SyntaxToken Previous()
    {
        return Tokens[Position - 1];
    }

    public SyntaxToken At(int position)
    {
        return Tokens[position];
    }

    public IEnumerable<SyntaxToken> Slice(int start, int length)
    {
        return Tokens.Skip(start).Take(length);
    }

    public bool IsMatch(params SyntaxTokenKind[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    public bool Check(SyntaxTokenKind type)
    {
        if (IsAtEnd())
        {
            return false;
        }

        return Peek().Kind == type;
    }

    public bool Check(params SyntaxTokenKind[] types)
    {
        if (IsAtEnd())
        {
            return false;
        }

        return types.Contains(Peek().Kind);
    }

    public SyntaxToken Expect(SyntaxTokenKind type, DiagnosticDescriptor descriptor)
    {
        if (Check(type))
        {
            return Advance();
        }

        var token = Peek();
        throw new ParseException(token.Span, descriptor);
    }

    public SyntaxToken Expect(SyntaxTokenKind type, DiagnosticDescriptor descriptor, Func<IPositionable, TextSpan> location)
    {
        if (Check(type))
        {
            return Advance();
        }

        throw new ParseException(location(Peek()), descriptor);
    }
}
