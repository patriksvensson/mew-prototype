namespace Mew.CodeAnalysis.Semantics;

public sealed class FunctionSymbolComparer : IEqualityComparer<FunctionSymbol>
{
    public bool Equals(FunctionSymbol? x, FunctionSymbol? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        if (!x.Name.Equals(y.Name, StringComparison.Ordinal))
        {
            return false;
        }

        if (x.ReturnType != y.ReturnType)
        {
            return false;
        }

        if (x.Parameters.Length != y.Parameters.Length)
        {
            return false;
        }

        for (var index = 0; index < x.Parameters.Length; index++)
        {
            if (x.Parameters[index].Type != y.Parameters[index].Type)
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode([DisallowNull] FunctionSymbol obj)
    {
        var builder = default(HashCode);
        builder.Add(obj.Name.GetHashCode());
        builder.Add(obj.ReturnType.GetHashCode());

        foreach (var param in obj.Parameters)
        {
            builder.Add(param.Type.Name.GetHashCode());
        }

        return builder.ToHashCode();
    }
}
