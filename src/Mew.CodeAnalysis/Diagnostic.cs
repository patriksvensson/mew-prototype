namespace Mew.CodeAnalysis;

[DebuggerDisplay("{Code,nq}: {Message,nq}")]
public sealed class Diagnostic
{
    public string Code { get; }
    public Location Location { get; }
    public string Message { get; }
    public Severity Severity { get; }
    public List<DiagnosticNote> Notes { get; }

    public Diagnostic(string code, Location location, Severity severity, string message)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        Severity = severity;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Notes = new List<DiagnosticNote>();
    }
}
