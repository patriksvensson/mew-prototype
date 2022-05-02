namespace Mew.Testing;

internal static class SyntaxTreeSerializer
{
    public static string Serialize(CompilationUnit program)
    {
        var builder = new Utf8StringWriter();
        using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { Indent = true }))
        {
            program.Accept(Visitor.Shared, writer);
        }

        return builder.ToString();
    }

    private sealed class Visitor : SyntaxVisitor<XmlWriter>
    {
        public static Visitor Shared { get; } = new Visitor();

        public override void VisitProgram(CompilationUnit syntax, XmlWriter context)
        {
            if (syntax.Statements.Length > 1)
            {
                context.WriteStartElement("Program");
            }

            base.VisitProgram(syntax, context);

            if (syntax.Statements.Length > 1)
            {
                context.WriteEndElement();
            }
        }

        public override void VisitExpression(ExpressionStatement syntax, XmlWriter writer)
        {
            syntax.Expression.Accept(this, writer);
        }

        public override void VisitLet(LetStatement syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Let");
            writer.WriteLocationAttributes(syntax);

            writer.WriteStartElement("Keyword");
            writer.WriteAttributeString("Type", syntax.Keyword.GetType().Name);
            writer.WriteLocationAttributes(syntax.Keyword);
            writer.WriteEndElement();

            writer.WriteStartElement("Identifier");
            writer.WriteAttributeString("Name", syntax.Name.IdentifierName);
            writer.WriteLocationAttributes(syntax.Name);
            writer.WriteEndElement();

            if (syntax.Initializer != null)
            {
                writer.WriteStartElement("Equal");
                writer.WriteAttributeString("Type", syntax.EqualToken!.GetType().Name);
                writer.WriteLocationAttributes(syntax.EqualToken!);
                writer.WriteEndElement();

                writer.WriteStartElement("Initializer");
                syntax.Initializer.Accept(this, writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public override void VisitBinary(BinaryExpression syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Binary");
            writer.WriteAttributeString("Operator", syntax.Operator.ToString());
            writer.WriteLocationAttributes(syntax);

            writer.WriteStartElement("Left");
            syntax.Left.Accept(this, writer);
            writer.WriteEndElement();

            writer.WriteStartElement("Right");
            syntax.Right.Accept(this, writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public override void VisitLogical(LogicalExpression syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Logical");
            writer.WriteAttributeString("Operator", syntax.Operator.ToString());
            writer.WriteLocationAttributes(syntax);

            writer.WriteStartElement("Left");
            syntax.Left.Accept(this, writer);
            writer.WriteEndElement();

            writer.WriteStartElement("Right");
            syntax.Right.Accept(this, writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public override void VisitGroup(GroupExpression syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Group");
            writer.WriteLocationAttributes(syntax);

            writer.WriteStartElement("Left");
            writer.WriteLocationAttributes(syntax.Left);
            writer.WriteEndElement();

            syntax.Expression.Accept(this, writer);

            writer.WriteStartElement("Right");
            writer.WriteLocationAttributes(syntax.Right);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public override void VisitIntegerLiteral(IntegerLiteralExpression syntax, XmlWriter writer)
        {
            writer.WriteStartElement("IntegerLiteral");
            writer.WriteAttributeString("Value", syntax.Value.ToString(CultureInfo.InvariantCulture));
            writer.WriteLocationAttributes(syntax);
            writer.WriteEndElement();
        }

        public override void VisitBooleanLiteral(BooleanLiteralExpression syntax, XmlWriter writer)
        {
            writer.WriteStartElement("BooleanLiteral");
            writer.WriteAttributeString("Value", syntax.Value.ToString(CultureInfo.InvariantCulture));
            writer.WriteLocationAttributes(syntax);
            writer.WriteEndElement();
        }

        public override void VisitAssignment(AssignmentExpression syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Assignment");
            writer.WriteLocationAttributes(syntax);

            writer.WriteStartElement("Identifier");
            writer.WriteAttributeString("Name", syntax.Name.IdentifierName);
            writer.WriteLocationAttributes(syntax.Name);
            writer.WriteEndElement();

            writer.WriteStartElement("Equal");
            writer.WriteLocationAttributes(syntax.EqualToken);
            writer.WriteEndElement();

            writer.WriteStartElement("Expression");
            syntax.Expression.Accept(this, writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public override void VisitRecovery(RecoverySyntax syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Skipped");
            writer.WriteLocationAttributes(syntax);

            if (syntax.Tokens.Count > 0)
            {
                writer.WriteStartElement("Tokens");
                foreach (var token in syntax.Tokens)
                {
                    token.Accept(this, writer);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public override void VisitToken(SyntaxToken syntax, XmlWriter context)
        {
        }

        public override void VisitIdentifier(IdentifierExpression syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Identifier");
            writer.WriteAttributeString("Lexeme", syntax.IdentifierName);
            writer.WriteLocationAttributes(syntax);
            writer.WriteEndElement();
        }

        public override void VisitIf(IfStatement syntax, XmlWriter writer)
        {
            writer.WriteStartElement("If");
            writer.WriteLocationAttributes(syntax);

            writer.WriteStartElement("Keyword");
            writer.WriteAttributeString("Type", syntax.Keyword.GetType().Name);
            writer.WriteLocationAttributes(syntax.Keyword);
            writer.WriteEndElement();

            writer.WriteStartElement("Then");
            syntax.ThenBranch.Accept(this, writer);
            writer.WriteEndElement();

            if (syntax.ElseBranch != null)
            {
                writer.WriteStartElement("Else");
                syntax.ElseBranch.Accept(this, writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public override void VisitBlock(BlockStatement syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Block");
            writer.WriteLocationAttributes(syntax);
            foreach (var statement in syntax.Statements)
            {
                statement.Accept(this, writer);
            }

            writer.WriteEndElement();
        }

        public override void VisitLoop(LoopStatement syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Loop");
            writer.WriteLocationAttributes(syntax);

            writer.WriteStartElement("Keyword");
            writer.WriteAttributeString("Type", syntax.Keyword.GetType().Name);
            writer.WriteLocationAttributes(syntax.Keyword);
            writer.WriteEndElement();

            writer.WriteStartElement("Body");
            writer.WriteLocationAttributes(syntax.Body);
            syntax.Body.Accept(this, writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public override void VisitBreak(BreakStatement syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Break");
            writer.WriteLocationAttributes(syntax);
            writer.WriteEndElement();
        }

        public override void VisitContinue(ContinueStatement syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Continue");
            writer.WriteLocationAttributes(syntax);
            writer.WriteEndElement();
        }

        public override void VisitUnary(UnaryExpression syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Unary");
            writer.WriteAttributeString("Operator", syntax.Operator.ToString());
            writer.WriteLocationAttributes(syntax);

            syntax.Expression.Accept(this, writer);

            writer.WriteEndElement();
        }

        public override void VisitFunctionCall(FunctionCallExpression syntax, XmlWriter writer)
        {
            writer.WriteStartElement("FunctionCall");
            writer.WriteLocationAttributes(syntax);

            writer.WriteStartElement("Callee");
            writer.WriteAttributeString("Name", syntax.Name.IdentifierName);
            writer.WriteLocationAttributes(syntax.Name);
            writer.WriteEndElement();

            if (syntax.Arguments != null)
            {
                writer.WriteStartElement("Arguments");
                foreach (var arg in syntax.Arguments)
                {
                    arg.Accept(this, writer);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public override void VisitStringLiteral(StringLiteralExpression syntax, XmlWriter writer)
        {
            writer.WriteStartElement("StringLiteral");
            writer.WriteAttributeString("Value", syntax.Value.ToString(CultureInfo.InvariantCulture));
            writer.WriteLocationAttributes(syntax);
            writer.WriteEndElement();
        }

        public override void VisitFunctionDeclaration(FunctionDeclarationStatement syntax, XmlWriter writer)
        {
            writer.WriteStartElement("FunctionDeclaration");

            writer.WriteLocationAttributes(syntax);

            writer.WriteStartElement("Name");
            writer.WriteAttributeString("Identifier", syntax.Name.IdentifierName);
            writer.WriteLocationAttributes(syntax.Name);
            writer.WriteEndElement();

            if (syntax.ReturnType != null)
            {
                writer.WriteStartElement("Returns");
                syntax.ReturnType.Accept(this, writer);
                writer.WriteEndElement();
            }

            if (syntax.Parameters != null)
            {
                writer.WriteStartElement("Parameters");
                foreach (var arg in syntax.Parameters)
                {
                    arg.Accept(this, writer);
                }

                writer.WriteEndElement();
            }

            writer.WriteStartElement("Body");
            writer.WriteLocationAttributes(syntax.Body);
            syntax.Body.Accept(this, writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public override void VisitParameter(ParameterSyntax syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Parameter");
            writer.WriteLocationAttributes(syntax);

            writer.WriteStartElement("Name");
            writer.WriteAttributeString("Identifier", syntax.Name.IdentifierName);
            writer.WriteLocationAttributes(syntax.Name);
            writer.WriteEndElement();

            writer.WriteStartElement("Type");
            writer.WriteLocationAttributes(syntax.Type);
            writer.WriteAttributeString("Identifier", syntax.Type.IdentifierName);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public override void VisitWhile(WhileStatement syntax, XmlWriter writer)
        {
            writer.WriteStartElement("While");
            writer.WriteLocationAttributes(syntax);

            writer.WriteStartElement("Keyword");
            writer.WriteAttributeString("Type", syntax.Keyword.GetType().Name);
            writer.WriteLocationAttributes(syntax.Keyword);
            writer.WriteEndElement();

            writer.WriteStartElement("Condition");
            writer.WriteLocationAttributes(syntax.Body);
            syntax.Condition.Accept(this, writer);
            writer.WriteEndElement();

            writer.WriteStartElement("Body");
            writer.WriteLocationAttributes(syntax.Body);
            syntax.Body.Accept(this, writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public override void VisitReturn(ReturnStatement syntax, XmlWriter writer)
        {
            writer.WriteStartElement("Return");
            writer.WriteLocationAttributes(syntax);

            syntax.Expression?.Accept(this, writer);

            writer.WriteEndElement();
        }

        public override void VisitExternalFunctionDeclaration(ExternalFunctionDeclarationStatement syntax, XmlWriter writer)
        {
            writer.WriteStartElement("ExternalFunctionDeclaration");

            writer.WriteLocationAttributes(syntax);

            writer.WriteStartElement("Name");
            writer.WriteAttributeString("Identifier", syntax.Name.IdentifierName);
            writer.WriteLocationAttributes(syntax.Name);
            writer.WriteEndElement();

            if (syntax.ReturnType != null)
            {
                writer.WriteStartElement("Returns");
                syntax.ReturnType.Accept(this, writer);
                writer.WriteEndElement();
            }

            if (syntax.Parameters != null)
            {
                writer.WriteStartElement("Parameters");
                foreach (var arg in syntax.Parameters)
                {
                    arg.Accept(this, writer);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
