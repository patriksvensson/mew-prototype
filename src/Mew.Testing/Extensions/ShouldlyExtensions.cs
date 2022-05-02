namespace Mew.Testing;

public static class ShouldlyExtensions
{
    public static T ShouldBeOfType<T>(this object obj, Action<T> action)
    {
        var t = obj.ShouldBeOfType<T>();
        action(t);
        return t;
    }

    public static T And<T>(this T obj)
    {
        return obj;
    }

    public static T And<T>(this T obj, Action<T> action)
    {
        action(obj);
        return obj;
    }
}
