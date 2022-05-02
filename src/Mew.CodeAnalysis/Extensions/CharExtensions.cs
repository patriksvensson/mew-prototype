namespace Mew.CodeAnalysis;

internal static class CharExtensions
{
    public static bool IsAlpha(this char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }

    public static bool IsAlphaNumeric(this char c)
    {
        return IsAlpha(c) || char.IsDigit(c);
    }
}
