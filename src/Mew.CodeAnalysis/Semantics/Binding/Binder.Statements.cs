namespace Mew.CodeAnalysis.Semantics;

internal static partial class Binder
{
    public static BoundStatement BindBlockStatement(BinderContext context, BlockStatement syntax)
    {
        var statements = ImmutableArray.CreateBuilder<BoundStatement>();

        context.RunInChildScope(() =>
        {
            foreach (var statement in syntax.Statements)
            {
                statements.Add(BindStatement(context, statement));
            }
        });

        return new BoundBlockStatement(syntax, statements.ToImmutable());
    }

    public static BoundStatement BindErrorStatement(Syntax syntax)
    {
        return new BoundExpressionStatement(syntax, new BoundErrorExpression(syntax));
    }

    public static BoundStatement BindExpressionStatement(BinderContext context, ExpressionStatement syntax)
    {
        var expression = BindExpression(context, syntax.Expression);
        return new BoundExpressionStatement(syntax, expression);
    }

    public static void BindExternalFunctionDeclaration(BinderContext context, ExternalFunctionDeclarationStatement syntax)
    {
        var parameters = ImmutableArray.CreateBuilder<ParameterSymbol>();

        foreach (var rawParameterSyntax in syntax.Parameters)
        {
            if (rawParameterSyntax is ParameterSyntax parameterSyntax)
            {
                var parameterName = parameterSyntax.Name.IdentifierName;
                var parameterType = BindType(context, parameterSyntax.Type);
                parameters.Add(new ParameterSymbol(parameterName, parameterSyntax, parameterType));
            }
        }

        var returnType = BindType(context, syntax.ReturnType as IdentifierExpression);
        if (syntax.ReturnType == null)
        {
            returnType = PrimitiveTypeSymbol.Void;
        }

        var function = new ExternalFunctionSymbol(
            syntax.Name.IdentifierName,
            syntax,
            parameters.ToImmutable(),
            returnType,
            "mewstd");

        if (!context.Scope.Functions.DeclareFunction(function))
        {
            context.AddDiagnostic(
                syntax, DiagnosticDescriptor.MEW1104_Function_Already_Declared(syntax.Name.IdentifierName));
        }
    }

    public static void BindFunctionDeclaration(BinderContext context, FunctionDeclarationStatement syntax)
    {
        var parameters = ImmutableArray.CreateBuilder<ParameterSymbol>();

        foreach (var rawParameterSyntax in syntax.Parameters)
        {
            if (rawParameterSyntax is ParameterSyntax parameterSyntax)
            {
                var parameterName = parameterSyntax.Name.IdentifierName;
                var parameterType = BindType(context, parameterSyntax.Type);
                parameters.Add(new ParameterSymbol(parameterName, parameterSyntax, parameterType));
            }
        }

        var returnType = BindType(context, syntax.ReturnType as IdentifierExpression);
        if (syntax.ReturnType == null)
        {
            returnType = PrimitiveTypeSymbol.Void;
        }

        var function = new DeclaredFunctionSymbol(
            syntax.Name.IdentifierName,
            syntax,
            parameters.ToImmutable(),
            returnType);

        if (!context.Scope.Functions.DeclareFunction(function))
        {
            context.AddDiagnostic(
                syntax, DiagnosticDescriptor.MEW1104_Function_Already_Declared(syntax.Name.IdentifierName));
        }
    }

    public static BoundStatement BindBreakStatement(BinderContext context, BreakStatement syntax)
    {
        if (!context.IsInLoop)
        {
            context.AddDiagnostic(
                syntax, DiagnosticDescriptor.MEW1112_Break_Used_Outside_Of_Loop);

            return BindErrorStatement(syntax);
        }

        return new BoundGotoStatement(syntax, context.BreakLabel);
    }

    public static BoundStatement BindContinueStatement(BinderContext context, ContinueStatement syntax)
    {
        if (!context.IsInLoop)
        {
            context.AddDiagnostic(
                syntax, DiagnosticDescriptor.MEW1113_Continue_Used_Outside_Of_Loop);

            return BindErrorStatement(syntax);
        }

        return new BoundGotoStatement(syntax, context.ContinueLabel);
    }

    public static BoundStatement BindIfStatement(BinderContext context, IfStatement syntax)
    {
        var condition = BindExpression(context, syntax.Condition);
        if (condition is BoundBooleanLiteralExpression boolean)
        {
            if (!boolean.Value)
            {
                context.AddDiagnostic(
                    syntax.ThenBranch, DiagnosticDescriptor.MEW1111_Unreachable_Code);
            }
            else if (syntax.ElseBranch != null)
            {
                context.AddDiagnostic(
                    syntax.ElseBranch, DiagnosticDescriptor.MEW1111_Unreachable_Code);
            }
        }

        var thenBranch = BindStatement(context, syntax.ThenBranch);
        var elseBranch = syntax.ElseBranch != null ? BindStatement(context, syntax.ElseBranch) : null;

        return new BoundIfStatement(syntax, condition, thenBranch, elseBranch);
    }

    public static BoundStatement BindLoopStatement(BinderContext context, LoopStatement syntax)
    {
        var body = context.BindLoop(context, syntax.Body, out var breakLabel, out var continueLabel);
        return new BoundLoopStatement(syntax, body, breakLabel, continueLabel);
    }

    public static BoundStatement BindReturnStatement(BinderContext context, ReturnStatement syntax)
    {
        var expression = syntax.Expression != null ? BindExpression(context, syntax.Expression) : null;

        if (context.Function != null)
        {
            // Got a return type?
            if (context.Function.ReturnType != PrimitiveTypeSymbol.Void)
            {
                // We're returning a value?
                if (expression != null)
                {
                    if (expression.Type != context.Function.ReturnType)
                    {
                        context.AddDiagnostic(
                            syntax, DiagnosticDescriptor.MEW1117_Wrong_Return_Type(context.Function.ReturnType, expression.Type));
                    }
                }
                else
                {
                    context.AddDiagnostic(
                        syntax, DiagnosticDescriptor.MEW1116_Missing_Return_Expression(context.Function.ReturnType));
                }
            }
            else
            {
                if (expression != null)
                {
                    context.AddDiagnostic(
                        syntax, DiagnosticDescriptor.MEW1115_Enclosing_Function_Does_Not_Return_A_Value);
                }
            }
        }

        return new BoundReturnStatement(syntax, expression);
    }

    public static BoundStatement BindVariableDeclaration(BinderContext context, LetStatement syntax)
    {
        var initializer = BindExpression(context, syntax.Initializer);
        var variableType = initializer.Type;

        var variable = new VariableSymbol(syntax.Name.IdentifierName, variableType);

        if (!context.Scope.TryDeclareVariable(variable))
        {
            context.AddDiagnostic(
                syntax.Name, DiagnosticDescriptor.MEW1103_Variable_Already_Declared(syntax.Name.IdentifierName));
        }

        return new BoundLetStatement(syntax, variable, initializer);
    }

    public static BoundStatement BindWhileStatement(BinderContext context, WhileStatement syntax)
    {
        var condition = BindExpression(context, syntax.Condition);

        if (condition is BoundBooleanLiteralExpression boolean)
        {
            if (!boolean.Value)
            {
                context.AddDiagnostic(
                    syntax.Body, DiagnosticDescriptor.MEW1111_Unreachable_Code);
            }
        }

        var body = context.BindLoop(context, syntax.Body, out var breakLabel, out var continueLabel);
        return new BoundWhileStatement(syntax, condition, body, breakLabel, continueLabel);
    }
}
