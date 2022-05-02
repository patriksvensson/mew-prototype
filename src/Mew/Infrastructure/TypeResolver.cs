namespace Mew.Infrastructure;

public sealed class TypeResolver : ITypeResolver
{
    private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider)
    {
        _provider = provider;
    }

    public object? Resolve(Type? type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return _provider.GetService(type);
    }
}