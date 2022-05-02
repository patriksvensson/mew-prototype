using System.Text;
using Mew.CodeAnalysis.Semantics;

namespace Mew.Testing;

public sealed class SemanticModelSerializer
{
    public static string Serialize(SemanticModel model)
    {
        var builder = new Utf8StringWriter();
        using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { Indent = true }))
        {
            writer.WriteStartElement("Statements");

            foreach (var statement in model.Statements)
            {
                statement.Accept(Visitor.Shared, writer);
            }

            writer.WriteEndElement();
        }

        return builder.ToString();
    }

    private sealed class Visitor : BoundNodeVisitor<XmlWriter>
    {
        public static Visitor Shared { get; } = new Visitor();

        public override void VisitLetStatement(BoundLetStatement node, XmlWriter context)
        {
            context.WriteStartElement("Let");
            context.WriteAttributeString("Name", node.Variable.Name);
            context.WriteAttributeString("Value", ExpressionPrinter.Print(node.Initializer));
            context.WriteEndElement();
        }

        public override void VisitLabelStatement(BoundLabelStatement node, XmlWriter context)
        {
            context.WriteStartElement("Label");
            context.WriteAttributeString("Name", node.Label.Name);
            context.WriteEndElement();
        }

        public override void VisitGotoStatement(BoundGotoStatement node, XmlWriter context)
        {
            context.WriteStartElement("Goto");
            context.WriteAttributeString("Name", node.Label.Name);
            context.WriteEndElement();
        }

        public override void VisitConditionalGotoStatement(BoundConditionalGotoStatement node, XmlWriter context)
        {
            context.WriteStartElement("ConditionalGoto");
            context.WriteAttributeString("If", ExpressionPrinter.Print(node.Condition));
            context.WriteAttributeString("Is", node.JumpIfTrue ? "True" : "False");
            context.WriteAttributeString("Goto", node.Label.Name);
            context.WriteEndElement();
        }

        public override void VisitFunctionCallExpression(BoundFunctionCallExpression node, XmlWriter context)
        {
            context.WriteStartElement("FunctionCall");
            context.WriteAttributeString("Name", node.Function.Name);

            if (node.Arguments.Length > 0)
            {
                foreach (var p in node.Arguments)
                {
                    context.WriteStartElement("Argument");
                    context.WriteAttributeString("Type", p.Type.Name);
                    context.WriteValue(ExpressionPrinter.Print(p));
                    context.WriteEndElement();
                }
            }

            context.WriteEndElement();
        }

        public override void VisitBinaryExpression(BoundBinaryExpression node, XmlWriter context)
        {
            context.WriteElementString("BinaryExpression", ExpressionPrinter.Print(node));
        }

        public override void VisitIntegerLiteralExpression(BoundIntegerLiteralExpression node, XmlWriter context)
        {
            context.WriteStartElement("Integer");
            context.WriteAttributeString("Value", node.Value.ToString());
            context.WriteEndElement();
        }
    }

    private sealed class ExpressionPrinter : BoundNodeVisitor<StringBuilder>
    {
        public static ExpressionPrinter Shared { get; } = new ExpressionPrinter();

        public static string Print(BoundExpression expression)
        {
            var builder = new StringBuilder();
            expression.Accept(Shared, builder);
            return builder.ToString();
        }

        public override void VisitBinaryExpression(BoundBinaryExpression node, StringBuilder context)
        {
            node.Left.Accept(this, context);
            context.Append(' ');

            context.Append(node.Op.Operator switch
            {
                BinaryOperator.Add => "+",
                BinaryOperator.Subtract => "-",
                BinaryOperator.Multiply => "*",
                BinaryOperator.Divide => "/",
                BinaryOperator.Modolu => "%",
                BinaryOperator.NotEqual => "!=",
                BinaryOperator.Equal => "==",
                BinaryOperator.GreaterThan => ">",
                BinaryOperator.GreaterThanOrEqual => ">=",
                BinaryOperator.LessThan => "<",
                BinaryOperator.LessThanOrEqual => "<=",
                _ => "?",
            });

            context.Append(' ');
            node.Right.Accept(this, context);
        }

        public override void VisitAssignment(BoundAssignmentExpression node, StringBuilder context)
        {
            context.Append(node.Variable.Name);
            context.Append(" = ");
            node.Expression.Accept(this, context);
        }

        public override void VisitVariableExpression(BoundVariableExpression node, StringBuilder context)
        {
            context.Append(node.Symbol.Name);
        }

        public override void VisitIntegerLiteralExpression(BoundIntegerLiteralExpression node, StringBuilder context)
        {
            context.Append(node.Value.ToString());
        }

        public override void VisitStringLiteralExpression(BoundStringLiteralExpression node, StringBuilder context)
        {
            context.Append(node.Value);
        }

        public override void VisitFunctionCallExpression(BoundFunctionCallExpression node, StringBuilder context)
        {
            context.Append(node.Function.Name + "(");
            context.Append(')');
        }
    }
}
