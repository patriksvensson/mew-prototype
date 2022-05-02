namespace Mew.CodeAnalysis;

public abstract class SyntaxVisitor<TContext> : ISyntaxVisitor<TContext>
{
    public virtual void VisitProgram(CompilationUnit syntax, TContext context)
    {
        foreach (var node in syntax.Statements)
        {
            Visit(node, context);
        }
    }

    public virtual void VisitAssignment(AssignmentExpression syntax, TContext context)
    {
        Visit(syntax.Name, context);
        Visit(syntax.EqualToken, context);
        Visit(syntax.Expression, context);
    }

    public virtual void VisitBinary(BinaryExpression syntax, TContext context)
    {
        Visit(syntax.Left, context);
        Visit(syntax.OperatorToken, context);
        Visit(syntax.Right, context);
    }

    public virtual void VisitBlock(BlockStatement syntax, TContext context)
    {
        Visit(syntax.LeftBrace, context);
        VisitNodes(syntax.Statements, context);
        Visit(syntax.RightBrace, context);
    }

    public virtual void VisitBooleanLiteral(BooleanLiteralExpression syntax, TContext context)
    {
        Visit(syntax.Token, context);
    }

    public virtual void VisitBreak(BreakStatement syntax, TContext context)
    {
        Visit(syntax.Keyword, context);
    }

    public virtual void VisitContinue(ContinueStatement syntax, TContext context)
    {
        Visit(syntax.Keyword, context);
    }

    public virtual void VisitExpression(ExpressionStatement syntax, TContext context)
    {
        Visit(syntax.Expression, context);
    }

    public virtual void VisitExternalFunctionDeclaration(ExternalFunctionDeclarationStatement syntax, TContext context)
    {
        Visit(syntax.ExternalKeyword, context);
        Visit(syntax.FunctionKeyword, context);
        Visit(syntax.Name, context);
        Visit(syntax.LeftParen, context);
        VisitNodes(syntax.Parameters, context);
        Visit(syntax.RightParen, context);
    }

    public virtual void VisitFunctionCall(FunctionCallExpression syntax, TContext context)
    {
        Visit(syntax.Name, context);
        Visit(syntax.LeftParen, context);
        VisitNodes(syntax.Arguments, context);
        Visit(syntax.RightParen, context);
    }

    public virtual void VisitFunctionDeclaration(FunctionDeclarationStatement syntax, TContext context)
    {
        Visit(syntax.Keyword, context);
        Visit(syntax.Name, context);
        Visit(syntax.LeftParen, context);
        VisitNodes(syntax.Parameters, context);
        Visit(syntax.RightParen, context);

        Visit(syntax.Arrow, context);
        Visit(syntax.ReturnType, context);

        Visit(syntax.Body, context);
    }

    public virtual void VisitGroup(GroupExpression syntax, TContext context)
    {
        Visit(syntax.Left, context);
        Visit(syntax.Expression, context);
        Visit(syntax.Right, context);
    }

    public virtual void VisitIdentifier(IdentifierExpression syntax, TContext context)
    {
        Visit(syntax.Child, context);
    }

    public virtual void VisitIf(IfStatement syntax, TContext context)
    {
        Visit(syntax.Keyword, context);
        Visit(syntax.Condition, context);
        Visit(syntax.ThenBranch, context);
        Visit(syntax.ElseToken, context);
        Visit(syntax.ElseBranch, context);
    }

    public virtual void VisitIntegerLiteral(IntegerLiteralExpression syntax, TContext context)
    {
        Visit(syntax.Token, context);
    }

    public virtual void VisitLet(LetStatement syntax, TContext context)
    {
        Visit(syntax.Keyword, context);
        Visit(syntax.Name, context);
        Visit(syntax.EqualToken, context);
        Visit(syntax.Initializer, context);
    }

    public virtual void VisitLogical(LogicalExpression syntax, TContext context)
    {
        Visit(syntax.Left, context);
        Visit(syntax.OperatorToken, context);
        Visit(syntax.Right, context);
    }

    public virtual void VisitLoop(LoopStatement syntax, TContext context)
    {
        Visit(syntax.Keyword, context);
        Visit(syntax.Body, context);
    }

    public virtual void VisitParameter(ParameterSyntax syntax, TContext context)
    {
        Visit(syntax.Name, context);
        Visit(syntax.Colon, context);
        Visit(syntax.Type, context);
    }

    public virtual void VisitRecovery(RecoverySyntax syntax, TContext context)
    {
        foreach (var token in syntax.Tokens)
        {
            Visit(token, context);
        }
    }

    public virtual void VisitReturn(ReturnStatement syntax, TContext context)
    {
        Visit(syntax.Keyword, context);
        Visit(syntax.Expression, context);
    }

    public virtual void VisitStringLiteral(StringLiteralExpression syntax, TContext context)
    {
        Visit(syntax.Token, context);
    }

    public virtual void VisitToken(SyntaxToken syntax, TContext context)
    {
        foreach (var trivia in syntax.LeadingTrivia)
        {
            VisitSyntaxTrivia(trivia, context);
        }

        foreach (var trivia in syntax.LeadingTrivia)
        {
            VisitSyntaxTrivia(trivia, context);
        }
    }

    public virtual void VisitUnary(UnaryExpression syntax, TContext context)
    {
        Visit(syntax.OperatorToken, context);
        Visit(syntax.Expression, context);
    }

    public virtual void VisitWhile(WhileStatement syntax, TContext context)
    {
        Visit(syntax.Keyword, context);
        Visit(syntax.Condition, context);
        Visit(syntax.Body, context);
    }

    protected virtual void VisitSyntaxTrivia(SyntaxTrivia trivia, TContext context)
    {
    }

    [DebuggerStepThrough]
    protected virtual void Visit(Syntax? syntax, TContext context)
    {
        if (syntax == null)
        {
            return;
        }

        RuntimeHelpers.EnsureSufficientExecutionStack();
        syntax.Accept(this, context);
    }

    [DebuggerStepThrough]
    protected void VisitTokens(IEnumerable<SyntaxToken> tokens, TContext context)
    {
        foreach (var token in tokens)
        {
            Visit(token, context);
        }
    }

    [DebuggerStepThrough]
    protected void VisitNodes(IEnumerable<Syntax> syntaxes, TContext context)
    {
        foreach (var node in syntaxes)
        {
            Visit(node, context);
        }
    }
}
