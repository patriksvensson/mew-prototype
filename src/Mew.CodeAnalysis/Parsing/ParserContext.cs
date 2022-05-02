namespace Mew.CodeAnalysis;

internal sealed class ParserContext
{
    private int _synchronizations;

    public SyntaxTree SyntaxTree { get; }
    public Diagnostics Diagnostics { get; }
    public SyntaxTokenReader Reader { get; }

    public ParserContext(SyntaxTree syntaxTree, IReadOnlyList<SyntaxToken> tokens)
    {
        SyntaxTree = syntaxTree ?? throw new ArgumentNullException(nameof(syntaxTree));
        Diagnostics = new Diagnostics();
        Reader = new SyntaxTokenReader(tokens);
    }

    public void AddDiagnostic(TextSpan span, DiagnosticDescriptor descriptor)
    {
        var location = new Location(SyntaxTree.Path, span);
        Diagnostics.Add(descriptor.ToDiagnostic(location));
    }

    public void Synchronize()
    {
        IncreaseSynchronizationCount();
        Synchronizer.Synchronize(this);
    }

    public Syntax WithRecovery<TSyntax>(Func<TSyntax> func, RecoveryFlag recovery, params SyntaxTokenKind[] terminators)
        where TSyntax : Syntax
    {
        var startPosition = Reader.Position;

        try
        {
            return func();
        }
        catch (ParseException exception)
        {
            var location = new Location(SyntaxTree.Path, exception.Span);
            var diagnostic = exception.Diagnostic.ToDiagnostic(location);

            IncreaseSynchronizationCount();
            return Synchronizer.SynchronizeAndReturnTrivia(
                this, startPosition, recovery,
                diagnostic, terminators);
        }
    }

    public IdentifierExpression IdentifierWithRecovery(DiagnosticDescriptor descriptor, RecoveryFlag flags, params SyntaxTokenKind[] terminatingTypes)
    {
        var identifierOrSkipped = WithRecovery(
            () =>
            {
                if (!Reader.IsAtEnd() && Keywords.Shared.IsKeyword(Reader.Current))
                {
                    throw new ParseException(
                        Reader.Current.Span,
                        DiagnosticDescriptor.MEW1030_Expected_Identifier_Found_Keyword(Reader.Current.Lexeme));
                }

                var identifier = Reader.Expect(SyntaxTokenKind.Identifier, descriptor);
                return new IdentifierExpression(SyntaxTree, identifier);
            },
            flags,
            terminatingTypes);

        switch (identifierOrSkipped)
        {
            case IdentifierExpression identifier:
                return identifier;

            case RecoverySyntax skipped:
                return new IdentifierExpression(SyntaxTree, skipped);

            default:
                throw new NotSupportedException($"Unexpected identifier syntax type '{identifierOrSkipped.GetType().Name}'");
        }
    }

    public RecoveryFlag GetSuppressionFlag(Syntax? precedingNode)
    {
        if (precedingNode == null)
        {
            return RecoveryFlag.None;
        }

        static RecoveryFlag ConvertFlags(bool suppress) => suppress
            ? RecoveryFlag.SuppressDiagnostics
            : RecoveryFlag.None;

        // When we have an incomplete declarations like "param\n",
        // the keyword is parsed but all other properties are set to 0-length SkippedTriviaSyntax
        // to prevent stacking multiple parse errors on a 0-length span (which is technically correct but also confusing)
        // we will only leave the first parse error.
        switch (precedingNode)
        {
            case IdentifierExpression identifier when !identifier.IsValid:
                return ConvertFlags(identifier.Span.Length == 0);

            case RecoverySyntax skipped:
                return ConvertFlags(skipped.Span.Length == 0);

            default:
                return ConvertFlags(!precedingNode.IsValid);
        }
    }

    private void IncreaseSynchronizationCount()
    {
        if (_synchronizations > 100)
        {
            throw new InvalidOperationException("Too many parse errors encountered");
        }

        _synchronizations++;
    }
}
