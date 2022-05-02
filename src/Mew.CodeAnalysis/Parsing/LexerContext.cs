namespace Mew.CodeAnalysis;

internal sealed class LexerContext
{
    private readonly TextBuffer _textBuffer;
    private readonly List<SyntaxToken> _tokens;
    private readonly SyntaxTree _syntaxTree;

    public int Start { get; set; }
    public int Position => _textBuffer.Position;
    public bool IsAtEnd => !_textBuffer.CanRead;
    public Diagnostics Diagnostics { get; }

    public string Path { get; }
    public IReadOnlyList<SyntaxToken> Tokens => _tokens;

    public LexerContext(SyntaxTree syntaxTree, string source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        _textBuffer = new TextBuffer(source);
        _tokens = new List<SyntaxToken>();
        _syntaxTree = syntaxTree ?? throw new ArgumentNullException(nameof(syntaxTree));

        Path = _syntaxTree.Path;
        Diagnostics = new Diagnostics();
    }

    public string SubString(int start, int length)
    {
        return _textBuffer.Slice(start, start + length).ToString();
    }

    public void AddError(DiagnosticDescriptor descriptor)
    {
        Diagnostics.Add(new Diagnostic(
            descriptor.Code,
            new Location(Path, new TextSpan(Start, Position - Start)),
            Severity.Error,
            descriptor.Message));
    }

    public void AddEof()
    {
        _tokens.Add(new SyntaxToken(
            _syntaxTree,
            new TextSpan(Position, 0),
            SyntaxTokenKind.Eof, string.Empty, null,
            ImmutableArray<SyntaxTrivia>.Empty,
            ImmutableArray<SyntaxTrivia>.Empty));
    }

    public void AddToken(SyntaxTokenKind type, string? literal, ImmutableArray<SyntaxTrivia> leadingTrivia, Func<ImmutableArray<SyntaxTrivia>> trailingTrivia)
    {
        var span = new TextSpan(Start, Position - Start);
        var lexeme = _textBuffer.Slice(Start, Position).ToString();
        var trailing = trailingTrivia();
        AddToken(new SyntaxToken(_syntaxTree, span, type, lexeme, literal, leadingTrivia, trailing));
    }

    public void AddToken(SyntaxToken token)
    {
        _tokens.Add(token);
    }

    public char Advance()
    {
        return _textBuffer.Read();
    }

    public bool IsMatch(char expected)
    {
        if (!_textBuffer.CanRead)
        {
            return false;
        }

        if (_textBuffer.Current != expected)
        {
            return false;
        }

        _textBuffer.Read();
        return true;
    }

    public char Peek(int offset = 0)
    {
        return _textBuffer.Peek(offset);
    }

    public char PeekNext()
    {
        return Peek(1);
    }
}
