namespace Mew.CodeAnalysis;

public interface ISyntaxVisitor<TContext>
{
    void VisitToken(SyntaxToken syntax, TContext context);
    void VisitRecovery(RecoverySyntax syntax, TContext context);
    void VisitParameter(ParameterSyntax syntax, TContext context);
    void VisitProgram(CompilationUnit syntax, TContext context);

    // Expressions
    void VisitAssignment(AssignmentExpression syntax, TContext context);
    void VisitBinary(BinaryExpression syntax, TContext context);
    void VisitBooleanLiteral(BooleanLiteralExpression syntax, TContext context);
    void VisitFunctionCall(FunctionCallExpression syntax, TContext context);
    void VisitGroup(GroupExpression syntax, TContext context);
    void VisitIdentifier(IdentifierExpression syntax, TContext context);
    void VisitIntegerLiteral(IntegerLiteralExpression syntax, TContext context);
    void VisitLogical(LogicalExpression syntax, TContext context);
    void VisitReturn(ReturnStatement syntax, TContext context);
    void VisitStringLiteral(StringLiteralExpression syntax, TContext context);
    void VisitUnary(UnaryExpression syntax, TContext context);

    // Statements
    void VisitBlock(BlockStatement syntax, TContext context);
    void VisitBreak(BreakStatement syntax, TContext context);
    void VisitContinue(ContinueStatement syntax, TContext context);
    void VisitExpression(ExpressionStatement syntax, TContext context);
    void VisitExternalFunctionDeclaration(ExternalFunctionDeclarationStatement syntax, TContext context);
    void VisitFunctionDeclaration(FunctionDeclarationStatement syntax, TContext context);
    void VisitIf(IfStatement syntax, TContext context);
    void VisitLet(LetStatement syntax, TContext context);
    void VisitLoop(LoopStatement syntax, TContext context);
    void VisitWhile(WhileStatement syntax, TContext context);
}
