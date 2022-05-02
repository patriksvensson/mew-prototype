namespace Mew.CodeAnalysis;

internal sealed class ParseException : Exception
{
    public TextSpan Span { get; }
    public DiagnosticDescriptor Diagnostic { get; }

    public ParseException(TextSpan span, DiagnosticDescriptor diagnostic)
    {
        Span = span;
        Diagnostic = diagnostic ?? throw new ArgumentNullException(nameof(diagnostic));
    }
}
