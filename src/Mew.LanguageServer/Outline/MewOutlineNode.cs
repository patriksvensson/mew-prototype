namespace Mew.LangServer.Outline;

public sealed class MewOutlineNode
{
    public string Name { get; }
    public string Kind { get; }
    public string Icon { get; }
    public TextSpan Span { get; }
    public List<MewOutlineNode> Children { get; }

    public MewOutlineNode(string name, string kind, string icon, TextSpan span)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Kind = kind ?? throw new ArgumentNullException(nameof(kind));
        Icon = icon ?? throw new ArgumentNullException(nameof(icon));
        Children = new List<MewOutlineNode>();
        Span = span;
    }
}
