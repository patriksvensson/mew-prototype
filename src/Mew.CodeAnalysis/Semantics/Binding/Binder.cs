namespace Mew.CodeAnalysis.Semantics;

internal static partial class Binder
{
    public static BoundStatement BindStatement(BinderContext context, Syntax syntax)
    {
        return syntax switch
        {
            BlockStatement stmt => BindBlockStatement(context, stmt),
            BreakStatement stmt => BindBreakStatement(context, stmt),
            ContinueStatement stmt => BindContinueStatement(context, stmt),
            ExpressionStatement stmt => BindExpressionStatement(context, stmt),
            IfStatement stmt => BindIfStatement(context, stmt),
            LetStatement stmt => BindVariableDeclaration(context, stmt),
            LoopStatement stmt => BindLoopStatement(context, stmt),
            RecoverySyntax stmt => new BoundExpressionStatement(stmt, new BoundErrorExpression(syntax)),
            ReturnStatement stmt => BindReturnStatement(context, stmt),
            WhileStatement stmt => BindWhileStatement(context, stmt),
            _ => throw new Exception($"Unexpected statement syntax {syntax.GetType().Name}"),
        };
    }

    public static BoundExpression BindExpression(BinderContext context, Syntax syntax)
    {
        return syntax switch
        {
            AssignmentExpression expr => BindAssignmentExpression(context, expr),
            BinaryExpression expr => BindBinaryExpression(context, expr),
            BooleanLiteralExpression expr => new BoundBooleanLiteralExpression(expr),
            FunctionCallExpression expr => BindFunctionCall(context, expr),
            GroupExpression expr => BindExpression(context, expr.Expression),
            IdentifierExpression expr => BindVariableExpression(context, expr),
            IntegerLiteralExpression expr => new BoundIntegerLiteralExpression(expr),
            LogicalExpression expr => BindLogicalExpression(context, expr),
            RecoverySyntax expr => new BoundErrorExpression(expr),
            StringLiteralExpression expr => new BoundStringLiteralExpression(expr),
            UnaryExpression expr => BindUnaryExpression(context, expr),
            _ => throw new Exception($"Unexpected expression syntax '{syntax.GetType().Name}'"),
        };
    }
}
