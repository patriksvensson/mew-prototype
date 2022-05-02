namespace Mew.CodeAnalysis;

[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed class Location
{
    public string Path { get; }
    public TextSpan Span { get; }

    public Location(string path, TextSpan span)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
        Span = span;
    }

    private string DebuggerDisplay()
    {
        if (string.IsNullOrWhiteSpace(Path))
        {
            return $"\"{Path}\" at {Span.Position} ({Span.Length} characters)";
        }

        return $"at {Span.Position} ({Span.Length} characters)";
    }
}