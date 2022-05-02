namespace Mew.CodeAnalysis;

internal static class Synchronizer
{
    public static void Synchronize(ParserContext context)
    {
        context.Reader.Advance();

        while (!context.Reader.IsAtEnd())
        {
            // End of a statement?
            if (context.Reader.Previous().Kind == SyntaxTokenKind.Semicolon)
            {
                return;
            }

            switch (context.Reader.Peek().Kind)
            {
                case SyntaxTokenKind.Fn:
                case SyntaxTokenKind.If:
                case SyntaxTokenKind.Break:
                case SyntaxTokenKind.Continue:
                case SyntaxTokenKind.Return:
                case SyntaxTokenKind.Loop:
                case SyntaxTokenKind.Let:
                case SyntaxTokenKind.While:
                    return;
            }

            context.Reader.Advance();
        }
    }

    public static Syntax SynchronizeAndReturnTrivia(
        ParserContext context, int startPosition,
        RecoveryFlag recovery, Diagnostic diagnostic,
        params SyntaxTokenKind[] terminators)
    {
        var startToken = context.Reader.At(startPosition);

        Synchronize(context, false, terminators);
        var skippedTokens = context.Reader.Slice(startPosition, context.Reader.Position - startPosition);
        var skippedSpan = TextSpan.SafeBetween(skippedTokens, startToken.Span.Position);

        if (!recovery.HasFlag(RecoveryFlag.SuppressDiagnostics))
        {
            context.Diagnostics.Add(diagnostic);
        }

        return new RecoverySyntax(context.SyntaxTree, skippedSpan, skippedTokens);
    }

    private static void Synchronize(ParserContext context, bool consumeTerminator, params SyntaxTokenKind[] terminators)
    {
        if (context.Reader.IsMatch(SyntaxTokenKind.Eof))
        {
            return;
        }

        // Look at the previous token and see if the trailing trivia
        // contained a new line. Kind of hackish, but whatever...
        if (terminators.Contains(SyntaxTokenKind.NewLine) && context.Reader.Position > 0)
        {
            if (context.Reader.Previous().TrailingTrivia.Any(t => t.Kind == SyntaxTriviaKind.NewLine))
            {
                return;
            }
        }

        while (!context.Reader.IsAtEnd())
        {
            if (consumeTerminator ? context.Reader.IsMatch(terminators) : context.Reader.Check(terminators))
            {
                return;
            }

            if (terminators.Contains(SyntaxTokenKind.NewLine) && context.Reader.Position > 0)
            {
                if (context.Reader.Current.TrailingTrivia.Any(t => t.Kind == SyntaxTriviaKind.NewLine))
                {
                    context.Reader.Advance();
                    return;
                }
            }

            context.Reader.Advance();
        }
    }
}
