namespace Mew.LangServer.Outline;

internal sealed class MewOutlineBuilder
{
    public static MewOutlineNode Build(MewBuffer buffer)
    {
        var result = new List<MewOutlineNode>();

        var semanticModel = buffer.Compilation.GetSemanticModel();
        foreach (var tree in semanticModel.SyntaxTrees)
        {
            foreach (var statement in tree.Root.Statements)
            {
                var ctx = new Visitor.Context();
                statement.Accept(Visitor.Shared, ctx);

                if (ctx.Root != null)
                {
                    result.Add(ctx.Root);
                }
            }
        }

        var span = result.Count > 0
            ? TextSpan.Between(result[0].Span, result.Last().Span)
            : new TextSpan(0, 0);

        var root = new MewOutlineNode("AST", string.Empty, "folder", span);
        root.Children.AddRange(result);
        return root;
    }

    private sealed class Visitor : SyntaxVisitor<Visitor.Context>
    {
        public static Visitor Shared { get; } = new Visitor();

        public sealed class Context
        {
            public MewOutlineNode? Root { get; private set; }
            public MewOutlineNode? Current { get; private set; }

            public void Push(MewOutlineNode node)
            {
                if (Root == null)
                {
                    Root = node;
                }

                Current = node;
            }

            public void Add(MewOutlineNode node)
            {
                if (Root == null)
                {
                    Root = node;
                }

                if (Current == null)
                {
                    Current = node;
                }
                else
                {
                    Current.Children.Add(node);
                }
            }
        }

        public override void VisitLet(LetStatement stmt, Context context)
        {
            var current = new MewOutlineNode("Let", stmt.Name.IdentifierName, "let", stmt.Span);
            context.Add(current);
            context.Push(current);

            if (stmt.Initializer != null)
            {
                var current2 = new MewOutlineNode("Target", "Expression", "dependency", stmt.Initializer.Span);
                current.Children.Add(current2);
                context.Push(current2);
                Visit(stmt.Initializer, context);
            }

            context.Push(current);
        }

        public override void VisitAssignment(AssignmentExpression expr, Context context)
        {
            var current = new MewOutlineNode("Assignment", expr.Name.IdentifierName, "number", expr.Span);
            context.Add(current);
            context.Push(current);

            var current2 = new MewOutlineNode("Target", "Expression", "dependency", expr.Expression.Span);
            current.Children.Add(current2);
            context.Push(current2);
            Visit(expr.Expression, context);

            context.Push(current);
        }

        public override void VisitIf(IfStatement stmt, Context context)
        {
            var current = new MewOutlineNode("If", "Statement", "dependency", stmt.Span);
            context.Add(current);

            var current2 = new MewOutlineNode("Then", "Branch", "dependency", stmt.ThenBranch.Span);
            current.Children.Add(current2);
            context.Push(current2);
            Visit(stmt.ThenBranch, context);

            if (stmt.ElseBranch != null)
            {
                var current3 = new MewOutlineNode("Else", "Branch", "dependency", stmt.ElseBranch.Span);
                current.Children.Add(current3);
                context.Push(current3);
                Visit(stmt.ElseBranch, context);
            }

            context.Push(current);
        }

        public override void VisitIdentifier(IdentifierExpression expr, Context context)
        {
            var current = new MewOutlineNode(expr.IdentifierName, "Identifier", "string", expr.Span);
            context.Add(current);
        }

        public override void VisitIntegerLiteral(IntegerLiteralExpression expr, Context context)
        {
            var current = new MewOutlineNode(expr.Token.Lexeme, "Number", "number", expr.Span);
            context.Add(current);
        }

        public override void VisitBooleanLiteral(BooleanLiteralExpression syntax, Context context)
        {
            var current = new MewOutlineNode(syntax.Token.Lexeme, "Boolean", "number", syntax.Span);
            context.Add(current);
        }

        public override void VisitBreak(BreakStatement stmt, Context context)
        {
            var current = new MewOutlineNode("Break", "Statement", "break", stmt.Span);
            context.Add(current);
        }

        public override void VisitContinue(ContinueStatement syntax, Context context)
        {
            var current = new MewOutlineNode("Continue", "Statement", "loop", syntax.Span);
            context.Add(current);
        }

        public override void VisitLoop(LoopStatement stmt, Context context)
        {
            var current = new MewOutlineNode("Loop", "Statement", "loop", stmt.Span);
            context.Add(current);

            Visit(stmt.Body, context);

            context.Push(current);
        }

        public override void VisitBlock(BlockStatement stmt, Context context)
        {
            var current = context.Current;
            if (current == null)
            {
                current = new MewOutlineNode("Block", "Statement", "dependency", stmt.Span);
                context.Push(current);
            }

            foreach (var statement in stmt.Statements)
            {
                statement.Accept(this, context);
                context.Push(current);
            }
        }

        public override void VisitRecovery(RecoverySyntax syntax, Context context)
        {
            var current = new MewOutlineNode("Recovery", "Statement", "recovery", syntax.Span);
            context.Push(current);

            foreach (var statement in syntax.Tokens)
            {
                statement.Accept(this, context);
                context.Push(current);
            }
        }

        public override void VisitStringLiteral(StringLiteralExpression expr, Context context)
        {
            var current = new MewOutlineNode(expr.Token.Lexeme, "String", "number", expr.Span);
            context.Add(current);
        }

        public override void VisitFunctionCall(FunctionCallExpression expr, Context context)
        {
            var current = new MewOutlineNode("FunctionCall", expr.Name.IdentifierName, "dependency", expr.Span);
            context.Add(current);
            context.Push(current);

            foreach (var arg in expr.Arguments)
            {
                Visit(arg, context);
            }
        }

        public override void VisitWhile(WhileStatement syntax, Context context)
        {
            var current = new MewOutlineNode("While", "Statement", "loop", syntax.Span);
            context.Add(current);
            context.Push(current);

            syntax.Body.Accept(this, context);
        }

        public override void VisitFunctionDeclaration(FunctionDeclarationStatement syntax, Context context)
        {
            var current = new MewOutlineNode("FunctionDeclaration", syntax.Name.IdentifierName, "dependency", syntax.Span);
            context.Add(current);
            context.Push(current);

            syntax.Body.Accept(this, context);
        }

        public override void VisitBinary(BinaryExpression syntax, Context context)
        {
            var current = new MewOutlineNode("Binary", syntax.Operator.ToString(), "dependency", syntax.Span);
            context.Add(current);
            context.Push(current);

            syntax.Left.Accept(this, context);
            context.Push(current);

            syntax.Right.Accept(this, context);
        }
    }
}
