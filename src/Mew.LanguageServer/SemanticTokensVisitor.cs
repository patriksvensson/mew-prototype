namespace Mew.LangServer;

public sealed class SemanticTokensVisitor : SyntaxVisitor<List<(IPositionable Token, SemanticTokenType Type)>>
{
    public static SemanticTokensVisitor Shared { get; } = new SemanticTokensVisitor();

    public override void VisitAssignment(AssignmentExpression syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Name.Accept(this, context);
        syntax.EqualToken.Accept(this, context);
        context.Add((syntax.Name, SemanticTokenType.Variable));
        context.Add((syntax.EqualToken, SemanticTokenType.Operator));

        syntax.Expression.Accept(this, context);
    }

    public override void VisitBinary(BinaryExpression syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Left.Accept(this, context);
        syntax.Right.Accept(this, context);

        syntax.OperatorToken.Accept(this, context);
        context.Add((syntax.OperatorToken, SemanticTokenType.Operator));
    }

    protected override void VisitSyntaxTrivia(SyntaxTrivia trivia, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        if (trivia.Kind == SyntaxTriviaKind.Comment)
        {
            context.Add((trivia, SemanticTokenType.Comment));
        }
    }

    public override void VisitBooleanLiteral(BooleanLiteralExpression syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Token.Accept(this, context);
        context.Add((syntax.Token, SemanticTokenType.Keyword));
    }

    public override void VisitBreak(BreakStatement syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Keyword.Accept(this, context);
        context.Add((syntax.Keyword, SemanticTokenType.Keyword));
    }

    public override void VisitContinue(ContinueStatement syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Keyword.Accept(this, context);
        context.Add((syntax.Keyword, SemanticTokenType.Keyword));
    }

    public override void VisitIntegerLiteral(IntegerLiteralExpression syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Token.Accept(this, context);
        context.Add((syntax.Token, SemanticTokenType.Number));
    }

    public override void VisitLogical(LogicalExpression syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Left.Accept(this, context);
        syntax.Right.Accept(this, context);

        syntax.OperatorToken.Accept(this, context);
        context.Add((syntax.OperatorToken, SemanticTokenType.Operator));
    }

    public override void VisitLet(LetStatement syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Keyword.Accept(this, context);
        syntax.Name.Accept(this, context);

        context.Add((syntax.Keyword, SemanticTokenType.Keyword));
        context.Add((syntax.Name, SemanticTokenType.Variable));

        if (syntax.EqualToken != null)
        {
            syntax.EqualToken.Accept(this, context);
            context.Add((syntax.EqualToken, SemanticTokenType.Operator));
        }

        syntax.Initializer?.Accept(this, context);
    }

    public override void VisitIdentifier(IdentifierExpression syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Child.Accept(this, context);
        context.Add((syntax, SemanticTokenType.Variable));
    }

    public override void VisitIf(IfStatement syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Keyword.Accept(this, context);
        context.Add((syntax.Keyword, SemanticTokenType.Keyword));

        syntax.Condition.Accept(this, context);
        syntax.ThenBranch.Accept(this, context);

        syntax.ElseBranch?.Accept(this, context);
        if (syntax.ElseToken != null)
        {
            syntax.ElseToken.Accept(this, context);
            context.Add((syntax.ElseToken, SemanticTokenType.Keyword));
        }
    }

    public override void VisitLoop(LoopStatement syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Keyword.Accept(this, context);
        context.Add((syntax.Keyword, SemanticTokenType.Keyword));

        syntax.Body.Accept(this, context);
    }

    public override void VisitFunctionCall(FunctionCallExpression syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Name.Accept(this, context);
        context.Add((syntax.Name, SemanticTokenType.Function));

        foreach (var arg in syntax.Arguments)
        {
            arg.Accept(this, context);
            context.Add((arg, SemanticTokenType.Parameter));
        }
    }

    public override void VisitStringLiteral(StringLiteralExpression syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Token.Accept(this, context);
        context.Add((syntax.Token, SemanticTokenType.String));
    }

    public override void VisitFunctionDeclaration(FunctionDeclarationStatement syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Keyword.Accept(this, context);
        context.Add((syntax.Keyword, SemanticTokenType.Keyword));

        syntax.Name.Accept(this, context);
        context.Add((syntax.Name, SemanticTokenType.Function));

        foreach (var parameter in syntax.Parameters)
        {
            parameter.Accept(this, context);
        }

        if (syntax.Arrow != null)
        {
            syntax.Arrow.Accept(this, context);
            context.Add((syntax.Arrow, SemanticTokenType.Operator));
        }

        if (syntax.ReturnType != null)
        {
            syntax.ReturnType.Accept(this, context);
            context.Add((syntax.ReturnType, SemanticTokenType.Type));
        }

        syntax.Body.Accept(this, context);
    }

    public override void VisitParameter(ParameterSyntax syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Name.Accept(this, context);
        context.Add((syntax.Name, SemanticTokenType.Parameter));

        syntax.Type.Accept(this, context);
        context.Add((syntax.Type, SemanticTokenType.Type));
    }

    public override void VisitWhile(WhileStatement syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Keyword.Accept(this, context);
        context.Add((syntax.Keyword, SemanticTokenType.Keyword));

        syntax.Condition.Accept(this, context);
        syntax.Body.Accept(this, context);
    }

    public override void VisitReturn(ReturnStatement syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.Keyword.Accept(this, context);
        context.Add((syntax.Keyword, SemanticTokenType.Keyword));

        syntax.Expression?.Accept(this, context);
    }

    public override void VisitExternalFunctionDeclaration(ExternalFunctionDeclarationStatement syntax, List<(IPositionable Token, SemanticTokenType Type)> context)
    {
        syntax.ExternalKeyword.Accept(this, context);
        context.Add((syntax.ExternalKeyword, SemanticTokenType.Keyword));

        syntax.FunctionKeyword.Accept(this, context);
        context.Add((syntax.FunctionKeyword, SemanticTokenType.Keyword));

        syntax.Name.Accept(this, context);
        context.Add((syntax.Name, SemanticTokenType.Function));

        foreach (var parameter in syntax.Parameters)
        {
            parameter.Accept(this, context);
        }

        if (syntax.Arrow != null)
        {
            syntax.Arrow.Accept(this, context);
            context.Add((syntax.Arrow, SemanticTokenType.Operator));
        }

        if (syntax.ReturnType != null)
        {
            syntax.ReturnType.Accept(this, context);
            context.Add((syntax.ReturnType, SemanticTokenType.Type));
        }
    }
}
