namespace Mew.Testing;

public static class XmlWriterExtensions
{
    public static void WriteLocationAttributes(this XmlWriter writer, Syntax syntax)
    {
        if (!string.IsNullOrWhiteSpace(syntax.SyntaxTree.Path))
        {
            writer.WriteAttributeString("Source", syntax.SyntaxTree.Path);
        }

        writer.WriteAttributeString("Position", syntax.Span.Position.ToString(CultureInfo.InvariantCulture));
        writer.WriteAttributeString("Length", syntax.Span.Length.ToString(CultureInfo.InvariantCulture));
    }
}
