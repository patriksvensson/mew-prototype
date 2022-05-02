using static Mew.CodeAnalysis.Semantics.BoundNodeBuilder;

namespace Mew.CodeAnalysis.Semantics;

public sealed class Lowerer
{
    public static BoundStatement Lower(BoundNode node)
    {
        var context = new Rewriter.Context();
        return node.Accept(Rewriter.Shared, context) switch
        {
            BoundStatement statement => statement,
            BoundExpression statement => new BoundExpressionStatement(statement.Syntax, statement),
            _ => throw new InvalidOperationException("Lowerer did not return a statement or expression"),
        };
    }

    private sealed class Rewriter : BoundNodeRewriter<Rewriter.Context>
    {
        public static Rewriter Shared { get; } = new Rewriter();

        public sealed class Context
        {
            private int _labelCounter;

            public Context()
            {
                _labelCounter = 1;
            }

            public BoundLabel CreateLabel()
            {
                return new BoundLabel($"Label{_labelCounter++}");
            }
        }

        public override BoundNode VisitIfStatement(BoundIfStatement node, Context context)
        {
            if (node.ElseBranch != null)
            {
                var elseLabel = context.CreateLabel();
                var endLabel = context.CreateLabel();

                var result = Block(
                    node.Syntax,
                    new BoundStatement[]
                    {
                        // gotoFalse <condition> else
                        // <then>
                        // goto end
                        // else:
                        // <else>
                        // end:
                        GotoFalse(node.Syntax, elseLabel, node.Condition),
                        node.ThenBranch,
                        Goto(node.Syntax, endLabel),
                        Label(node.Syntax, elseLabel),
                        node.ElseBranch,
                        Label(node.Syntax, endLabel),
                    });

                return result.Accept(this, context);
            }
            else
            {
                var endLabel = context.CreateLabel();
                var result = Block(
                    node.Syntax,
                    new BoundStatement[]
                    {
                        // gotoFalse <condition> end
                        // <then>
                        // end:
                        GotoFalse(node.Syntax, endLabel, node.Condition),
                        node.ThenBranch,
                        Label(node.Syntax, endLabel),
                    });

                return result.Accept(this, context);
            }
        }

        public override BoundNode VisitWhileStatement(BoundWhileStatement node, Context context)
        {
            var bodyLabel = context.CreateLabel();
            var result = Block(
                    node.Syntax,
                    new BoundStatement[]
                    {
                        // goto continue
                        // body:
                        // <body>
                        // continue:
                        // gotoTrue <condition> body
                        // break:
                        Goto(node.Syntax, node.ContinueLabel),
                        Label(node.Syntax, bodyLabel),
                        node.Body,
                        Label(node.Syntax, node.ContinueLabel),
                        GotoTrue(node.Syntax, bodyLabel, node.Condition),
                        Label(node.Syntax, node.BreakLabel),
                    });

            return result.Accept(this, context);
        }
    }
}