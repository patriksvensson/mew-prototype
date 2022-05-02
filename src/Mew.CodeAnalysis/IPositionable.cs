namespace Mew.CodeAnalysis;

public interface IPositionable
{
    TextSpan Span { get; }
}