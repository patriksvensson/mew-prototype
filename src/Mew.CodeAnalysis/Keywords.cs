namespace Mew.CodeAnalysis;

internal sealed class Keywords
{
    private readonly Dictionary<string, SyntaxTokenKind> _tokenTypes;
    private readonly List<string> _all;

    public static Keywords Shared { get; } = new Keywords();
    public IReadOnlyList<string> All => _all;

    public Keywords()
    {
        _tokenTypes = new Dictionary<string, SyntaxTokenKind>()
        {
            { Constants.Keywords.And, SyntaxTokenKind.And },
            { Constants.Keywords.Break, SyntaxTokenKind.Break },
            { Constants.Keywords.Continue, SyntaxTokenKind.Continue },
            { Constants.Keywords.Else, SyntaxTokenKind.Else },
            { Constants.Keywords.Extern, SyntaxTokenKind.Extern },
            { Constants.Keywords.False, SyntaxTokenKind.False },
            { Constants.Keywords.Fn, SyntaxTokenKind.Fn },
            { Constants.Keywords.If, SyntaxTokenKind.If },
            { Constants.Keywords.Let, SyntaxTokenKind.Let },
            { Constants.Keywords.Loop, SyntaxTokenKind.Loop },
            { Constants.Keywords.Or, SyntaxTokenKind.Or },
            { Constants.Keywords.Return, SyntaxTokenKind.Return },
            { Constants.Keywords.True, SyntaxTokenKind.True },
            { Constants.Keywords.While, SyntaxTokenKind.While },
        };

        _all = new List<string>(_tokenTypes.Keys);
    }

    public bool IsKeyword(SyntaxToken? token)
    {
        if (token == null)
        {
            return false;
        }

        return _tokenTypes.Values.Any(x => x == token.Kind);
    }

    public bool TryGetTokenType(string name, [NotNullWhen(true)] out SyntaxTokenKind? type)
    {
        if (_tokenTypes.TryGetValue(name, out var result))
        {
            type = result;
            return true;
        }

        type = null;
        return false;
    }
}
