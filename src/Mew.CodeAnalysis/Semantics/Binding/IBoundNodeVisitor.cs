namespace Mew.CodeAnalysis.Semantics;

public interface IBoundNodeVisitor<TContext>
{
    // Expressions
    void VisitAssignment(BoundAssignmentExpression node, TContext context);
    void VisitBinaryExpression(BoundBinaryExpression node, TContext context);
    void VisitBooleanLiteralExpression(BoundBooleanLiteralExpression node, TContext context);
    void VisitConversionExpression(BoundConversionExpression node, TContext context);
    void VisitErrorExpression(BoundErrorExpression node, TContext context);
    void VisitFunctionCallExpression(BoundFunctionCallExpression node, TContext context);
    void VisitIntegerLiteralExpression(BoundIntegerLiteralExpression node, TContext context);
    void VisitLogicalExpression(BoundLogicalExpression node, TContext context);
    void VisitStringLiteralExpression(BoundStringLiteralExpression node, TContext context);
    void VisitUnaryExpression(BoundUnaryExpression node, TContext context);
    void VisitVariableExpression(BoundVariableExpression node, TContext context);

    // Statements
    void VisitBlockStatement(BoundBlockStatement node, TContext context);
    void VisitConditionalGotoStatement(BoundConditionalGotoStatement node, TContext context);
    void VisitExpressionStatement(BoundExpressionStatement node, TContext context);
    void VisitGotoStatement(BoundGotoStatement node, TContext context);
    void VisitIfStatement(BoundIfStatement node, TContext context);
    void VisitLabelStatement(BoundLabelStatement node, TContext context);
    void VisitLetStatement(BoundLetStatement node, TContext context);
    void VisitLoopStatement(BoundLoopStatement node, TContext context);
    void VisitReturnStatement(BoundReturnStatement node, TContext context);
    void VisitWhileStatement(BoundWhileStatement node, TContext context);
}

public interface IBoundNodeVisitor<TContext, TResult>
{
    // Expressions
    TResult VisitAssignment(BoundAssignmentExpression node, TContext context);
    TResult VisitBinaryExpression(BoundBinaryExpression node, TContext context);
    TResult VisitBooleanLiteralExpression(BoundBooleanLiteralExpression node, TContext context);
    TResult VisitConversionExpression(BoundConversionExpression node, TContext context);
    TResult VisitErrorExpression(BoundErrorExpression node, TContext context);
    TResult VisitFunctionCallExpression(BoundFunctionCallExpression node, TContext context);
    TResult VisitIntegerLiteralExpression(BoundIntegerLiteralExpression node, TContext context);
    TResult VisitLogicalExpression(BoundLogicalExpression node, TContext context);
    TResult VisitStringLiteralExpression(BoundStringLiteralExpression node, TContext context);
    TResult VisitUnaryExpression(BoundUnaryExpression node, TContext context);
    TResult VisitVariableExpression(BoundVariableExpression node, TContext context);

    // Statements
    TResult VisitBlockStatement(BoundBlockStatement node, TContext context);
    TResult VisitConditionalGotoStatement(BoundConditionalGotoStatement node, TContext context);
    TResult VisitExpressionStatement(BoundExpressionStatement node, TContext context);
    TResult VisitGotoStatement(BoundGotoStatement node, TContext context);
    TResult VisitIfStatement(BoundIfStatement node, TContext context);
    TResult VisitLabelStatement(BoundLabelStatement node, TContext context);
    TResult VisitLetStatement(BoundLetStatement node, TContext context);
    TResult VisitLoopStatement(BoundLoopStatement node, TContext context);
    TResult VisitReturnStatement(BoundReturnStatement node, TContext context);
    TResult VisitWhileStatement(BoundWhileStatement node, TContext context);
}