namespace Mew.CodeAnalysis;

internal sealed class Lexer
{
    public IReadOnlyList<SyntaxToken> Scan(SyntaxTree syntaxTree, string source, out Diagnostics diagnostics)
    {
        var context = new LexerContext(syntaxTree, source);

        while (!context.IsAtEnd)
        {
            context.Start = context.Position;
            ScanToken(context);
        }

        if (context.Tokens.Count == 0 || context.Tokens.Last().Kind != SyntaxTokenKind.Eof)
        {
            context.AddEof();
        }

        diagnostics = context.Diagnostics;
        return context.Tokens;
    }

    private void AddToken(LexerContext context, SyntaxTokenKind type, ImmutableArray<SyntaxTrivia> leadingTrivia, string? literal = null)
    {
        context.AddToken(type, literal, leadingTrivia, () => ScanTrivia(context, false));
    }

    private void ScanToken(LexerContext context)
    {
        var leadingTrivia = ScanTrivia(context, true);

        if (context.IsAtEnd)
        {
            AddToken(context, SyntaxTokenKind.Eof, leadingTrivia);
            return;
        }

        var c = context.Advance();
        switch (c)
        {
            case '(': AddToken(context, SyntaxTokenKind.LParen, leadingTrivia); break;
            case ')': AddToken(context, SyntaxTokenKind.RParen, leadingTrivia); break;
            case '{': AddToken(context, SyntaxTokenKind.LBrace, leadingTrivia); break;
            case '}': AddToken(context, SyntaxTokenKind.RBrace, leadingTrivia); break;
            case ',': AddToken(context, SyntaxTokenKind.Comma, leadingTrivia); break;
            case '.': AddToken(context, SyntaxTokenKind.Dot, leadingTrivia); break;
            case '-':
                AddToken(
                    context,
                    context.IsMatch('>') ? SyntaxTokenKind.Arrow : SyntaxTokenKind.Minus,
                    leadingTrivia);
                break;
            case '+': AddToken(context, SyntaxTokenKind.Plus, leadingTrivia); break;
            case ':': AddToken(context, SyntaxTokenKind.Colon, leadingTrivia); break;
            case ';': AddToken(context, SyntaxTokenKind.Semicolon, leadingTrivia); break;
            case '*': AddToken(context, SyntaxTokenKind.Star, leadingTrivia); break;
            case '%': AddToken(context, SyntaxTokenKind.Percent, leadingTrivia); break;
            case '!':
                AddToken(
                    context,
                    context.IsMatch('=') ? SyntaxTokenKind.BangEqual : SyntaxTokenKind.Bang,
                    leadingTrivia);
                break;
            case '=':
                AddToken(
                    context,
                    context.IsMatch('=') ? SyntaxTokenKind.EqualEqual : SyntaxTokenKind.Equal,
                    leadingTrivia);
                break;
            case '<':
                AddToken(
                    context,
                    context.IsMatch('=') ? SyntaxTokenKind.LessEqual : SyntaxTokenKind.Less,
                    leadingTrivia);
                break;
            case '>':
                AddToken(
                    context,
                    context.IsMatch('=') ? SyntaxTokenKind.GreaterEqual : SyntaxTokenKind.Greater,
                    leadingTrivia);
                break;
            case '|':
                if (context.IsMatch('|'))
                {
                    AddToken(context, SyntaxTokenKind.Or, leadingTrivia);
                }
                else
                {
                    context.AddError(DiagnosticDescriptor.MEW2004_Unexpected_Character('|'));
                }

                break;
            case '&':
                if (context.IsMatch('&'))
                {
                    AddToken(context, SyntaxTokenKind.And, leadingTrivia);
                }
                else
                {
                    context.AddError(DiagnosticDescriptor.MEW2004_Unexpected_Character('&'));
                }

                break;
            case '/':
                AddToken(context, SyntaxTokenKind.Slash, leadingTrivia);
                break;
            case '\"':
                ScanString(context, leadingTrivia);
                break;
            default:
                if (char.IsDigit(c))
                {
                    ScanNumber(context, leadingTrivia);
                }
                else if (c.IsAlpha())
                {
                    ScanIdentifier(context, leadingTrivia);
                }
                else
                {
                    context.AddError(DiagnosticDescriptor.MEW2004_Unexpected_Character(c));
                    AddToken(context, SyntaxTokenKind.Unrecognized, leadingTrivia);
                }

                break;
        }
    }

    private ImmutableArray<SyntaxTrivia> ScanTrivia(LexerContext context, bool leading)
    {
        var trivia = new List<SyntaxTrivia>();

        while (true)
        {
            var peek = context.Peek();

            if (leading && peek == '/' && context.PeekNext() == '/')
            {
                var start = context.Position;
                while (context.Peek() != '\r' && context.Peek() != '\n' && !context.IsAtEnd)
                {
                    context.Advance();
                }

                trivia.Add(new SyntaxTrivia(
                    new TextSpan(start, context.Position - start),
                    SyntaxTriviaKind.Comment,
                    context.SubString(start, context.Position - start)));
            }
            else if (peek == '\r' || peek == '\n')
            {
                var start = context.Position;
                while (!context.IsAtEnd)
                {
                    peek = context.Peek();
                    if (peek == '\r' || peek == '\n')
                    {
                        context.Advance();
                    }
                    else
                    {
                        break;
                    }
                }

                trivia.Add(new SyntaxTrivia(
                    new TextSpan(start, context.Position - start),
                    SyntaxTriviaKind.NewLine,
                    context.SubString(start, context.Position - start)));
            }
            else if (peek == ' ' || peek == '\t')
            {
                var start = context.Position;
                while (!context.IsAtEnd)
                {
                    peek = context.Peek();
                    if (peek == ' ' || peek == '\t')
                    {
                        context.Advance();
                    }
                    else
                    {
                        break;
                    }
                }

                trivia.Add(new SyntaxTrivia(
                    new TextSpan(start, context.Position - start),
                    SyntaxTriviaKind.Whitespace,
                    context.SubString(start, context.Position - start)));
            }
            else
            {
                break;
            }
        }

        // Update the start position
        context.Start = context.Position;

        // Leading trivia is white space and comments
        return ImmutableArray.CreateRange(trivia);
    }

    private void ScanString(LexerContext context, ImmutableArray<SyntaxTrivia> leadingTrivia)
    {
        while (context.Peek() != '\"' && !context.IsAtEnd)
        {
            if (context.Peek() == '\n' || context.Peek() == '\r')
            {
                break;
            }

            context.Advance();
        }

        if (context.IsAtEnd || context.Peek() != '\"')
        {
            context.AddError(DiagnosticDescriptor.MEW1005_Unterminated_String_Literal);
            return;
        }

        var value = context.SubString(context.Start + 1, context.Position - context.Start - 1);

        context.Advance(); // The closing "
        AddToken(context, SyntaxTokenKind.String, leadingTrivia, value);
    }

    private void ScanNumber(LexerContext context, ImmutableArray<SyntaxTrivia> leadingTrivia)
    {
        while (char.IsDigit(context.Peek()))
        {
            context.Advance();
        }

        var isDouble = false;
        if (context.Peek() == '.' && char.IsDigit(context.PeekNext()))
        {
            isDouble = true;
            context.Advance(); // Consume the "."

            while (char.IsDigit(context.Peek()))
            {
                context.Advance();
            }
        }

        AddToken(
            context,
            isDouble ? SyntaxTokenKind.Double : SyntaxTokenKind.Integer,
            leadingTrivia,
            context.SubString(context.Start, context.Position - context.Start));
    }

    private void ScanIdentifier(LexerContext context, ImmutableArray<SyntaxTrivia> leadingTrivia)
    {
        while (context.Peek().IsAlphaNumeric())
        {
            context.Advance();
        }

        var lexeme = context.SubString(context.Start, context.Position - context.Start);
        if (!Keywords.Shared.TryGetTokenType(lexeme, out var type))
        {
            type = SyntaxTokenKind.Identifier;
        }

        AddToken(context, type.Value, leadingTrivia, lexeme);
    }
}
