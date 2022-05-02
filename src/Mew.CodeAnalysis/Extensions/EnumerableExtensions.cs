namespace Mew.CodeAnalysis;

internal static class EnumerableExtensions
{
    public static List<T> AsList<T>(this IEnumerable<T> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (source is List<T> list)
        {
            return list;
        }

        return source.ToList();
    }
}
