namespace Mew.CodeAnalysis.Semantics;

internal static class Flattener
{
    public static BoundBlockStatement Flatten(BoundStatement node)
    {
        var builder = ImmutableArray.CreateBuilder<BoundStatement>();
        var stack = new Stack<BoundStatement>();
        stack.Push(node);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            if (current is BoundBlockStatement block)
            {
                foreach (var statement in block.Statements.Reverse())
                {
                    stack.Push(statement);
                }
            }
            else
            {
                builder.Add(current);
            }
        }

        return new BoundBlockStatement(node.Syntax, builder.ToImmutable());
    }

    public static ImmutableArray<BoundStatement> Flatten(IEnumerable<BoundStatement> nodes)
    {
        var builder = ImmutableArray.CreateBuilder<BoundStatement>();
        var stack = new Stack<BoundStatement>();

        foreach (var node in nodes.Reverse())
        {
            stack.Push(node);
        }

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            if (current is BoundBlockStatement block)
            {
                foreach (var statement in block.Statements.Reverse())
                {
                    stack.Push(statement);
                }
            }
            else
            {
                builder.Add(current);
            }
        }

        return builder.ToImmutable();
    }
}
