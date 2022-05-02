namespace Mew.CodeAnalysis;

internal sealed class Parser
{
    private readonly Lexer _lexer;

    public Parser()
    {
        _lexer = new Lexer();
    }

    public (CompilationUnit Root, Diagnostics Diagnostics) Parse(SyntaxTree tree)
    {
        var tokens = _lexer.Scan(tree, tree.Source, out var lexerDiagnostics);
        var nodes = Parse(tree, tokens, out var diagnostics);

        var unit = new CompilationUnit(tree, nodes);
        diagnostics = diagnostics.Merge(lexerDiagnostics);

        return (unit, diagnostics);
    }

    private List<StatementSyntax> Parse(SyntaxTree syntaxTree, IReadOnlyList<SyntaxToken> tokens, out Diagnostics diagnostics)
    {
        var result = new List<StatementSyntax>();
        var context = new ParserContext(syntaxTree, tokens);

        while (!context.Reader.IsAtEnd())
        {
            try
            {
                var declaration = ParseDeclaration(context);
                if (declaration != null)
                {
                    result.Add(declaration);
                }
            }
            catch (ParseException ex)
            {
                context.AddDiagnostic(ex.Span, ex.Diagnostic);
                context.Synchronize();
            }
        }

        diagnostics = context.Diagnostics;
        return result;
    }

    private StatementSyntax? ParseDeclaration(ParserContext context)
    {
        if (context.Reader.IsMatch(SyntaxTokenKind.Extern))
        {
            var externKeyword = context.Reader.Previous();

            if (!context.Reader.Check(SyntaxTokenKind.Fn))
            {
                context.AddDiagnostic(context.Reader.Current.Span, DiagnosticDescriptor.MEW1036_Expected_Function_Keyword);
                return null;
            }

            context.Reader.IsMatch(SyntaxTokenKind.Fn);
            var functionKeyword = context.Reader.Previous();

            return ParseFunctionDeclaration(context, externKeyword, functionKeyword);
        }

        if (context.Reader.IsMatch(SyntaxTokenKind.Fn))
        {
            var functionKeyword = context.Reader.Previous();
            return ParseFunctionDeclaration(context, null, functionKeyword);
        }

        if (context.Reader.IsMatch(SyntaxTokenKind.Let))
        {
            return ParseLetStatement(context);
        }

        return ParseStatement(context);
    }

    private StatementSyntax ParseFunctionDeclaration(ParserContext context, SyntaxToken? externKeyword, SyntaxToken functionKeyword)
    {
        var name = context.IdentifierWithRecovery(
            DiagnosticDescriptor.MEW1023_Expected_Function_Name,
            context.GetSuppressionFlag(functionKeyword),
            SyntaxTokenKind.Equal, SyntaxTokenKind.NewLine);

        var leftParen = context.WithRecovery(
            () => context.Reader.Expect(SyntaxTokenKind.LParen, DiagnosticDescriptor.MEW1010_Expected_LParam_After_Function_Name),
            context.GetSuppressionFlag(name),
            SyntaxTokenKind.Semicolon,
            SyntaxTokenKind.NewLine);

        var parameters = new List<Syntax>();
        if (!context.Reader.IsAtEnd() && !context.Reader.Check(SyntaxTokenKind.RParen))
        {
            do
            {
                var parameterName = context.IdentifierWithRecovery(
                    DiagnosticDescriptor.MEW1012_Expected_Parameter_Name,
                    context.GetSuppressionFlag(parameters.LastOrDefault() ?? leftParen),
                    SyntaxTokenKind.Equal, SyntaxTokenKind.NewLine, SyntaxTokenKind.Comma);

                var colon = context.WithRecovery(
                    () => context.Reader.Expect(SyntaxTokenKind.Colon, DiagnosticDescriptor.MEW1017_Expected_Colon),
                    context.GetSuppressionFlag(parameterName),
                    SyntaxTokenKind.Semicolon,
                    SyntaxTokenKind.Comma,
                    SyntaxTokenKind.NewLine);

                var parameterType = context.IdentifierWithRecovery(
                    DiagnosticDescriptor.MEW1022_Expected_Type,
                    context.GetSuppressionFlag(colon),
                    SyntaxTokenKind.Equal, SyntaxTokenKind.NewLine, SyntaxTokenKind.Comma);

                parameters.Add(new ParameterSyntax(context.SyntaxTree, parameterName, colon, parameterType));
            }
            while (context.Reader.IsMatch(SyntaxTokenKind.Comma));
        }

        var rightParen = context.WithRecovery(
            () => context.Reader.Expect(SyntaxTokenKind.RParen, DiagnosticDescriptor.MEW1003_Expected_RParen),
            context.GetSuppressionFlag(parameters.LastOrDefault() ?? leftParen),
            SyntaxTokenKind.Semicolon,
            SyntaxTokenKind.NewLine);

        var arrowToken = default(SyntaxToken?);
        var returnType = default(Syntax?);
        if (context.Reader.IsMatch(SyntaxTokenKind.Arrow))
        {
            arrowToken = context.Reader.Previous();
            returnType = context.IdentifierWithRecovery(
                DiagnosticDescriptor.MEW1022_Expected_Type,
                context.GetSuppressionFlag(rightParen),
                SyntaxTokenKind.Equal, SyntaxTokenKind.NewLine, SyntaxTokenKind.Comma);
        }

        // External function?
        if (externKeyword != null)
        {
            var semicolon = context.WithRecovery(
                () => context.Reader.Expect(SyntaxTokenKind.Semicolon, DiagnosticDescriptor.MEW1000_Expected_Semicolon),
                context.GetSuppressionFlag(returnType ?? arrowToken ?? rightParen),
                SyntaxTokenKind.NewLine);

            return new ExternalFunctionDeclarationStatement(
                context.SyntaxTree,
                externKeyword, functionKeyword, name, leftParen,
                ImmutableArray.CreateRange(parameters),
                rightParen, arrowToken, returnType);
        }

        var body = context.WithRecovery(
            () => ParseBlockStatement(context, rightParen),
            context.GetSuppressionFlag(rightParen),
            SyntaxTokenKind.Semicolon,
            SyntaxTokenKind.NewLine);

        return new FunctionDeclarationStatement(
            context.SyntaxTree,
            functionKeyword, name, leftParen,
            ImmutableArray.CreateRange(parameters),
            rightParen, arrowToken, returnType,
            body);
    }

    private LetStatement ParseLetStatement(ParserContext context)
    {
        var letToken = context.Reader.Previous();

        var name = context.IdentifierWithRecovery(
            DiagnosticDescriptor.MEW1001_Expected_Variable_Name,
            RecoveryFlag.None,
            SyntaxTokenKind.Equal, SyntaxTokenKind.NewLine);

        var assignment = context.WithRecovery(
            () => context.Reader.Expect(SyntaxTokenKind.Equal, DiagnosticDescriptor.MEW1018_Expected_Equal),
            context.GetSuppressionFlag(name),
            SyntaxTokenKind.Semicolon,
            SyntaxTokenKind.NewLine);

        var initializer = context.WithRecovery(
            () => ParseExpression(context),
            context.GetSuppressionFlag(assignment),
            SyntaxTokenKind.Semicolon,
            SyntaxTokenKind.NewLine);

        var semicolon = context.WithRecovery(
            () => context.Reader.Expect(
                SyntaxTokenKind.Semicolon,
                DiagnosticDescriptor.MEW1000_Expected_Semicolon),
            context.GetSuppressionFlag(initializer),
            SyntaxTokenKind.NewLine);

        return new LetStatement(context.SyntaxTree, letToken, name, assignment, initializer, semicolon);
    }

    private StatementSyntax ParseStatement(ParserContext context)
    {
        if (context.Reader.IsMatch(SyntaxTokenKind.If))
        {
            return ParseIf(context);
        }

        if (context.Reader.Check(SyntaxTokenKind.LBrace))
        {
            return ParseBlockStatement(context, null);
        }

        if (context.Reader.IsMatch(SyntaxTokenKind.Loop))
        {
            return ParseLoop(context);
        }

        if (context.Reader.IsMatch(SyntaxTokenKind.While))
        {
            return ParseWhile(context);
        }

        if (context.Reader.IsMatch(SyntaxTokenKind.Break))
        {
            return ParseBreak(context);
        }

        if (context.Reader.IsMatch(SyntaxTokenKind.Continue))
        {
            return ParseContinue(context);
        }

        if (context.Reader.IsMatch(SyntaxTokenKind.Return))
        {
            return ParseReturn(context);
        }

        return ParseExpressionStatement(context);
    }

    private StatementSyntax ParseIf(ParserContext context)
    {
        var ifToken = context.Reader.Previous();

        var condition = context.WithRecovery(
            () => ParseExpression(context),
            RecoveryFlag.None,
            SyntaxTokenKind.Semicolon,
            SyntaxTokenKind.NewLine);

        var thenBranch = context.WithRecovery(
            () => ParseStatement(context),
            context.GetSuppressionFlag(condition),
            SyntaxTokenKind.Semicolon,
            SyntaxTokenKind.NewLine);

        var elseToken = default(SyntaxToken?);
        var elseBranch = default(Syntax?);
        if (context.Reader.IsMatch(SyntaxTokenKind.Else))
        {
            elseToken = context.Reader.Previous();
            elseBranch = context.WithRecovery(
                () => ParseStatement(context),
                context.GetSuppressionFlag(thenBranch),
                SyntaxTokenKind.Semicolon,
                SyntaxTokenKind.NewLine);
        }

        return new IfStatement(context.SyntaxTree, ifToken, condition, thenBranch, elseToken, elseBranch);
    }

    private StatementSyntax ParseLoop(ParserContext context)
    {
        var keyword = context.Reader.Previous();

        var body = context.WithRecovery(
            () => ParseBlockStatement(context, keyword),
            RecoveryFlag.None,
            SyntaxTokenKind.Semicolon,
            SyntaxTokenKind.NewLine);

        return new LoopStatement(context.SyntaxTree, keyword, body);
    }

    private StatementSyntax ParseWhile(ParserContext context)
    {
        var keyword = context.Reader.Previous();

        var condition = context.WithRecovery(
            () => ParseExpression(context),
            RecoveryFlag.None,
            SyntaxTokenKind.Semicolon,
            SyntaxTokenKind.NewLine);

        var body = context.WithRecovery(
            () => ParseBlockStatement(context, condition),
            RecoveryFlag.None,
            SyntaxTokenKind.Semicolon,
            SyntaxTokenKind.NewLine);

        return new WhileStatement(context.SyntaxTree, keyword, condition, body);
    }

    private StatementSyntax ParseBreak(ParserContext context)
    {
        var keyword = context.Reader.Previous();

        var semicolon = context.WithRecovery(
            () => context.Reader.Expect(
                SyntaxTokenKind.Semicolon,
                DiagnosticDescriptor.MEW1000_Expected_Semicolon),
            RecoveryFlag.None,
            SyntaxTokenKind.NewLine);

        return new BreakStatement(context.SyntaxTree, keyword);
    }

    private StatementSyntax ParseContinue(ParserContext context)
    {
        var keyword = context.Reader.Previous();

        var semicolon = context.WithRecovery(
            () => context.Reader.Expect(
                SyntaxTokenKind.Semicolon,
                DiagnosticDescriptor.MEW1000_Expected_Semicolon),
            RecoveryFlag.None,
            SyntaxTokenKind.NewLine);

        return new ContinueStatement(context.SyntaxTree, keyword);
    }

    private StatementSyntax ParseReturn(ParserContext context)
    {
        var keyword = context.Reader.Previous();

        var value = default(Syntax?);
        if (context.Reader.Peek()?.Kind != SyntaxTokenKind.Semicolon)
        {
            value = context.WithRecovery(
                () => ParseExpression(context),
                RecoveryFlag.None,
                SyntaxTokenKind.NewLine);
        }

        var semicolon = context.WithRecovery(
            () => context.Reader.Expect(
                SyntaxTokenKind.Semicolon,
                DiagnosticDescriptor.MEW1000_Expected_Semicolon),
            context.GetSuppressionFlag(value),
            SyntaxTokenKind.NewLine);

        return new ReturnStatement(context.SyntaxTree, keyword, value);
    }

    private BlockStatement ParseBlockStatement(ParserContext context, Syntax? previous)
    {
        var lbrace = context.WithRecovery(
            () => context.Reader.Expect(SyntaxTokenKind.LBrace, DiagnosticDescriptor.MEW1020_Expected_LBrace_Before_Block),
            context.GetSuppressionFlag(previous),
            SyntaxTokenKind.NewLine);

        var statements = new List<Syntax>();
        while (!context.Reader.Check(SyntaxTokenKind.RBrace) && !context.Reader.IsAtEnd())
        {
            var statement = context.WithRecovery(
                () => ParseDeclaration(context)!,
                context.GetSuppressionFlag(statements.LastOrDefault() ?? lbrace),
                SyntaxTokenKind.NewLine);

            if (statement != null)
            {
                statements.Add(statement);
            }
        }

        var rbrace = context.WithRecovery(
            () => context.Reader.Expect(SyntaxTokenKind.RBrace, DiagnosticDescriptor.MEW1008_Expected_RBrace_After_Block, _ => lbrace.Span),
            context.GetSuppressionFlag(statements.Count > 0 ? statements.Last() : lbrace),
            SyntaxTokenKind.NewLine);

        return new BlockStatement(context.SyntaxTree, lbrace, statements, rbrace);
    }

    private ExpressionStatement ParseExpressionStatement(ParserContext context)
    {
        var expr = ParseExpression(context);

        var semicolon = context.WithRecovery(
            () => context.Reader.Expect(SyntaxTokenKind.Semicolon, DiagnosticDescriptor.MEW1000_Expected_Semicolon, _ => expr.Span),
            context.Diagnostics.Count > 0 ? RecoveryFlag.SuppressDiagnostics : RecoveryFlag.None,
            SyntaxTokenKind.NewLine);

        return new ExpressionStatement(context.SyntaxTree, expr);
    }

    private ExpressionSyntax ParseExpression(ParserContext context)
    {
        return ParseAssignment(context);
    }

    private ExpressionSyntax ParseAssignment(ParserContext context)
    {
        var expr = ParseOr(context);

        if (context.Reader.IsMatch(SyntaxTokenKind.Equal))
        {
            var equals = context.Reader.Previous();

            var right = context.WithRecovery(
                () => ParseAssignment(context),
                context.GetSuppressionFlag(expr),
                SyntaxTokenKind.Semicolon,
                SyntaxTokenKind.NewLine);

            if (expr is IdentifierExpression identifier)
            {
                return new AssignmentExpression(context.SyntaxTree, identifier, equals, right);
            }

            throw new ParseException(right.Span, DiagnosticDescriptor.MEW1007_Invalid_Assignment_Target);
        }

        return expr;
    }

    private ExpressionSyntax ParseOr(ParserContext context)
    {
        var expr = ParseAnd(context);

        while (context.Reader.IsMatch(SyntaxTokenKind.Or))
        {
            var @operator = context.Reader.Previous();
            var right = ParseAnd(context);
            expr = new LogicalExpression(context.SyntaxTree, expr, @operator, right);
        }

        return expr;
    }

    private ExpressionSyntax ParseAnd(ParserContext context)
    {
        var expr = ParseEquality(context);

        while (context.Reader.IsMatch(SyntaxTokenKind.And))
        {
            var @operator = context.Reader.Previous();
            var right = ParseAnd(context);
            expr = new LogicalExpression(context.SyntaxTree, expr, @operator, right);
        }

        return expr;
    }

    private ExpressionSyntax ParseEquality(ParserContext context)
    {
        var expr = ParseComparison(context);
        while (context.Reader.IsMatch(SyntaxTokenKind.BangEqual, SyntaxTokenKind.EqualEqual))
        {
            var @operator = context.Reader.Previous();
            var right = ParseComparison(context);
            expr = new BinaryExpression(context.SyntaxTree, expr, @operator, right);
        }

        return expr;
    }

    private ExpressionSyntax ParseComparison(ParserContext context)
    {
        var expr = ParseTerm(context);

        while (context.Reader.IsMatch(SyntaxTokenKind.Greater, SyntaxTokenKind.GreaterEqual, SyntaxTokenKind.Less, SyntaxTokenKind.LessEqual))
        {
            var @operator = context.Reader.Previous();
            var right = ParseTerm(context);
            expr = new BinaryExpression(context.SyntaxTree, expr, @operator, right);
        }

        return expr;
    }

    private ExpressionSyntax ParseTerm(ParserContext context)
    {
        var expr = ParseFactor(context);

        while (context.Reader.IsMatch(SyntaxTokenKind.Minus, SyntaxTokenKind.Plus))
        {
            var @operator = context.Reader.Previous();
            var right = ParseFactor(context);
            return new BinaryExpression(context.SyntaxTree, expr, @operator, right);
        }

        return expr;
    }

    private ExpressionSyntax ParseFactor(ParserContext context)
    {
        var expr = ParseUnary(context);

        while (context.Reader.IsMatch(SyntaxTokenKind.Slash, SyntaxTokenKind.Star, SyntaxTokenKind.Percent))
        {
            var @operator = context.Reader.Previous();
            var right = ParseUnary(context);
            return new BinaryExpression(context.SyntaxTree, expr, @operator, right);
        }

        return expr;
    }

    private ExpressionSyntax ParseUnary(ParserContext context)
    {
        if (context.Reader.IsMatch(SyntaxTokenKind.Bang) || context.Reader.IsMatch(SyntaxTokenKind.Minus))
        {
            var @operator = context.Reader.Previous();
            var right = context.WithRecovery(
                () => ParseUnary(context),
                RecoveryFlag.None,
                SyntaxTokenKind.RBrace,
                SyntaxTokenKind.RParen,
                SyntaxTokenKind.NewLine);

            return new UnaryExpression(context.SyntaxTree, @operator, right);
        }

        return ParseCall(context);
    }

    private ExpressionSyntax ParseCall(ParserContext context)
    {
        var expr = ParsePrimary(context);

        if (expr is IdentifierExpression callee)
        {
            while (true)
            {
                if (context.Reader.IsMatch(SyntaxTokenKind.LParen))
                {
                    expr = ParseFunctionCall(context, callee);
                }
                else
                {
                    break;
                }
            }
        }

        return expr;
    }

    private ExpressionSyntax ParseFunctionCall(ParserContext context, IdentifierExpression callee)
    {
        var lparen = context.Reader.Previous();

        var arguments = new List<Syntax>();
        if (!context.Reader.Check(SyntaxTokenKind.RParen))
        {
            do
            {
                var syntax = context.WithRecovery(
                    () => ParseExpression(context),
                    context.GetSuppressionFlag(callee),
                    SyntaxTokenKind.Comma,
                    SyntaxTokenKind.NewLine);

                arguments.Add(syntax);
            }
            while (context.Reader.IsMatch(SyntaxTokenKind.Comma));
        }

        var rparen = context.WithRecovery(
            () => context.Reader.Expect(SyntaxTokenKind.RParen, DiagnosticDescriptor.MEW1003_Expected_RParen),
            context.GetSuppressionFlag(arguments.Count > 0 ? arguments.Last() : callee),
            SyntaxTokenKind.NewLine);

        return new FunctionCallExpression(context.SyntaxTree, callee, lparen, rparen, ImmutableArray.CreateRange(arguments));
    }

    private ExpressionSyntax ParsePrimary(ParserContext context)
    {
        var nextToken = context.Reader.Current;

        // True?
        if (context.Reader.IsMatch(SyntaxTokenKind.True))
        {
            var token = context.Reader.Previous();
            return new BooleanLiteralExpression(context.SyntaxTree, token, true);
        }

        // False?
        if (context.Reader.IsMatch(SyntaxTokenKind.False))
        {
            var token = context.Reader.Previous();
            return new BooleanLiteralExpression(context.SyntaxTree, token, false);
        }

        // Integer?
        if (context.Reader.IsMatch(SyntaxTokenKind.Integer))
        {
            var token = context.Reader.Previous();
            return new IntegerLiteralExpression(
                context.SyntaxTree,
                token, long.Parse(token.Literal!, CultureInfo.InvariantCulture));
        }

        // String?
        if (context.Reader.IsMatch(SyntaxTokenKind.String))
        {
            var token = context.Reader.Previous();
            return new StringLiteralExpression(
                context.SyntaxTree, token, token.Literal!);
        }

        // Group?
        if (context.Reader.IsMatch(SyntaxTokenKind.LParen))
        {
            var left = context.Reader.Previous();
            var expr = ParseExpression(context);
            var right = context.Reader.Expect(SyntaxTokenKind.RParen, DiagnosticDescriptor.MEW1003_Expected_RParen,
                _ => TextSpan.Between(left, expr));

            return new GroupExpression(context.SyntaxTree, left, expr, right);
        }

        // Identifier?
        if (context.Reader.IsMatch(SyntaxTokenKind.Identifier))
        {
            return new IdentifierExpression(context.SyntaxTree, context.Reader.Previous());
        }

        throw new ParseException(nextToken.Span, DiagnosticDescriptor.MEW1019_Unrecognized_Expression);
    }
}
