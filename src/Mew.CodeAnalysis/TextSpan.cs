namespace Mew.CodeAnalysis;

[DebuggerDisplay("Pos={Position,nq}, Len={Length,nq}")]
public struct TextSpan
{
    public int Position { get; }
    public int Length { get; }

    public TextSpan(int position, int length)
    {
        Position = position;
        Length = length;
    }

    public bool Contains(int offset)
    {
        return offset >= Position && offset <= Position + Length;
    }

    public static TextSpan Between(TextSpan first, TextSpan second)
    {
        if (first.Position <= second.Position)
        {
            return new TextSpan(first.Position, second.Position + second.Length - first.Position);
        }

        return Between(second, first);
    }

    public static TextSpan Between<T>(IList<T> positionables)
        where T : IPositionable
    {
        if (positionables.Count == 0)
        {
            return new TextSpan(0, 0);
        }

        if (positionables.Count == 1)
        {
            return positionables[0].Span;
        }

        return Between(positionables.First(), positionables.Last());
    }

    public static TextSpan SafeBetween(IEnumerable<IPositionable> positionables, int fallbackPosition)
    {
        return positionables.Any()
            ? Between(positionables.First(), positionables.Last())
            : new TextSpan(fallbackPosition, 0);
    }

    public static TextSpan Between(IPositionable first, IPositionable? second)
    {
        if (second == null)
        {
            return first.Span;
        }

        if (first.Span.Position <= second.Span.Position)
        {
            return new TextSpan(
                first.Span.Position,
                second.Span.Position + second.Span.Length - first.Span.Position);
        }

        return Between(second, first);
    }
}
