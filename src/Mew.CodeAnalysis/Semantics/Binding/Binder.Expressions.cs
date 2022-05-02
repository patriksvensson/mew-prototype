namespace Mew.CodeAnalysis.Semantics;

internal static partial class Binder
{
    public static BoundExpression BindAssignmentExpression(BinderContext context, AssignmentExpression syntax)
    {
        var expression = BindExpression(context, syntax.Expression);

        var variable = BindVariableReference(context, syntax.Name);
        if (variable == null)
        {
            return expression;
        }

        return new BoundAssignmentExpression(syntax, variable, expression);
    }

    public static BoundExpression BindBinaryExpression(BinderContext context, BinaryExpression syntax)
    {
        var boundLeft = BindExpression(context, syntax.Left);
        if (boundLeft.Type == TypeSymbol.Error)
        {
            return new BoundErrorExpression(syntax);
        }

        var boundRight = BindExpression(context, syntax.Right);
        if (boundRight.Type == TypeSymbol.Error)
        {
            return new BoundErrorExpression(syntax);
        }

        var boundOperator = BoundBinaryOperator.Bind(syntax.Operator, boundLeft.Type, boundRight.Type);
        if (boundOperator == null)
        {
            context.AddDiagnostic(
                syntax.OperatorToken, DiagnosticDescriptor.MEW1105_Binary_Operator_Not_Defined(
                    syntax.OperatorToken.Lexeme,
                    boundLeft,
                    boundRight));

            return new BoundErrorExpression(syntax);
        }

        return new BoundBinaryExpression(syntax, boundLeft, boundOperator, boundRight);
    }

    public static BoundExpression BindFunctionCall(BinderContext context, FunctionCallExpression syntax)
    {
        // Bind arguments
        var boundArguments = ImmutableArray.CreateBuilder<BoundExpression>();
        foreach (var argument in syntax.Arguments)
        {
            boundArguments.Add(BindExpression(context, argument));
        }

        // Make sure the symbol is a function
        if (context.Scope.TryLookupSymbol(syntax.Name.IdentifierName, out var _))
        {
            context.AddDiagnostic(
                syntax, DiagnosticDescriptor.MEW1108_Not_A_Function(
                    syntax.Name.IdentifierName));

            return new BoundErrorExpression(syntax);
        }

        // No function at all with this name?
        if (!context.Scope.Functions.HasFunction(syntax.Name.IdentifierName))
        {
            context.AddDiagnostic(
                syntax, DiagnosticDescriptor.MEW1107_Undefined_Function(
                    syntax.Name.IdentifierName));

            return new BoundErrorExpression(syntax);
        }

        // Look up the function
        if (!context.Scope.Functions.TryGetExact(
                syntax.Name.IdentifierName,
                boundArguments.ToImmutable(),
                out var symbol))
        {
            // No overload with the same count?
            if (!context.Scope.Functions.TryGetWithArity(
                syntax.Name.IdentifierName,
                boundArguments.Count,
                out symbol))
            {
                context.AddDiagnostic(
                    syntax, DiagnosticDescriptor.MEW1118_No_Overload_With_Argument_Count(
                        syntax.Name.IdentifierName, boundArguments.Count));

                return new BoundErrorExpression(syntax);
            }
            else
            {
                // Try get the closest one
                if (!context.Scope.Functions.TryGetClosest(
                    syntax.Name.IdentifierName,
                    boundArguments.ToImmutable(),
                    out symbol))
                {
                    context.AddDiagnostic(
                        syntax, DiagnosticDescriptor.MEW1107_Undefined_Function(
                            syntax.Name.IdentifierName));

                    return new BoundErrorExpression(syntax);
                }
            }
        }

        // Not the same number of arguments?
        // Should not really happen, but let's check anyway.
        if (syntax.Arguments.Length != symbol.Parameters.Length)
        {
            context.AddDiagnostic(
                syntax, DiagnosticDescriptor.MEW1109_Invalid_Parameter_Count(
                    syntax.Name.IdentifierName,
                    symbol.Parameters.Length,
                    syntax.Arguments.Length));

            return new BoundErrorExpression(syntax);
        }

        // Make sure to convert items that need to be converted
        for (var index = 0; index < syntax.Arguments.Length; index++)
        {
            // Ignore recovery syntax
            if (syntax.Arguments[index] is not ExpressionSyntax)
            {
                continue;
            }

            // Ignore binding errors
            if (boundArguments[index].Type == TypeSymbol.Error)
            {
                continue;
            }

            // TODO: Need to figure out implicit conversion
            if (boundArguments[index].Type != symbol.Parameters[index].Type)
            {
                context.AddDiagnostic(
                    boundArguments[index].Syntax, DiagnosticDescriptor.MEW1110_Cannot_Convert_Implicitly(
                        boundArguments[index].Type,
                        symbol.Parameters[index].Type));
            }
        }

        return new BoundFunctionCallExpression(syntax, symbol, boundArguments.ToImmutable());
    }

    public static BoundExpression BindLogicalExpression(BinderContext context, LogicalExpression syntax)
    {
        var boundLeft = BindExpression(context, syntax.Left);
        var boundRight = BindExpression(context, syntax.Right);

        var boundOperator = BoundLogicalOperator.Bind(syntax.Operator, boundLeft.Type, boundRight.Type);
        if (boundOperator == null)
        {
            context.AddDiagnostic(
                syntax, DiagnosticDescriptor.MEW1106_Logical_Operator_Not_Defined(
                    syntax.OperatorToken.Lexeme,
                    boundLeft,
                    boundRight));

            return new BoundErrorExpression(syntax);
        }

        return new BoundLogicalExpression(syntax, boundLeft, boundOperator, boundRight);
    }

    public static TypeSymbol BindType(BinderContext context, IdentifierExpression? syntax)
    {
        if (syntax == null)
        {
            return TypeSymbol.Error;
        }

        if (context.Scope.Types.TryGet(syntax.IdentifierName, out var type))
        {
            return type;
        }

        context.AddDiagnostic(
            syntax, DiagnosticDescriptor.MEW1102_Unknown_Type(syntax.IdentifierName));

        // Return a type symbol for the type
        return new UndeclaredTypeSymbol(syntax.IdentifierName);
    }

    public static BoundExpression BindUnaryExpression(BinderContext context, UnaryExpression syntax)
    {
        var boundOperand = BindExpression(context, syntax.Expression);
        if (boundOperand.Type == TypeSymbol.Error)
        {
            return new BoundErrorExpression(syntax);
        }

        var boundOperator = BoundUnaryOperator.Bind(syntax.Operator, boundOperand.Type);
        if (boundOperator == null)
        {
            context.AddDiagnostic(
                syntax, DiagnosticDescriptor.MEW1114_Unary_Operator_Not_Defined(
                    syntax.OperatorToken.Lexeme,
                    boundOperand.Type));

            return new BoundErrorExpression(syntax);
        }

        return new BoundUnaryExpression(syntax, boundOperand, boundOperator);
    }

    public static BoundExpression BindVariableExpression(BinderContext context, IdentifierExpression syntax)
    {
        if (!syntax.IsValid)
        {
            return new BoundErrorExpression(syntax);
        }

        var symbol = BindVariableReference(context, syntax);
        if (symbol == null)
        {
            return new BoundErrorExpression(syntax);
        }

        return new BoundVariableExpression(syntax, symbol);
    }

    public static VariableSymbol? BindVariableReference(BinderContext context, IdentifierExpression syntax)
    {
        if (syntax.IsValid)
        {
            // Is this a function?
            if (context.Scope.Functions.HasFunction(syntax.IdentifierName))
            {
                context.AddDiagnostic(
                    syntax, DiagnosticDescriptor.MEW1101_Not_A_Variable(syntax.IdentifierName));

                return null;
            }

            if (context.Scope.TryLookupSymbol(syntax.IdentifierName, out var symbol))
            {
                if (symbol is VariableSymbol variable)
                {
                    return variable;
                }
                else
                {
                    context.AddDiagnostic(
                        syntax, DiagnosticDescriptor.MEW1101_Not_A_Variable(syntax.IdentifierName));
                }
            }
            else
            {
                context.AddDiagnostic(
                    syntax, DiagnosticDescriptor.MEW1100_Undeclared_Variable(syntax.IdentifierName));
            }
        }

        return null;
    }
}
