namespace Mew.CodeAnalysis;

[DebuggerDisplay("{Message,nq} ({Location,nq})")]
public sealed class DiagnosticNote
{
    public Location Location { get; }
    public string Message { get; }

    public DiagnosticNote(Location location, string message)
    {
        Location = location ?? throw new ArgumentNullException(nameof(location));
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}
