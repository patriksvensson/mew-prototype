using Mew.CodeAnalysis.Semantics;

namespace Mew.Testing;

public static class VerifierEx
{
    public static Task VerifySyntaxTree(SyntaxTree parsed)
    {
        parsed.ShouldNotBeNull();

        var xml = SyntaxTreeSerializer.Serialize(parsed.Root);
        var settings = new VerifySettings();
        settings.UseExtension("xml");
        return Verify(xml, settings);
    }

    public static Task VerifySemanticModel(SemanticModel model)
    {
        model.ShouldNotBeNull();

        var xml = SemanticModelSerializer.Serialize(model);
        var settings = new VerifySettings();
        settings.UseExtension("xml");
        return Verify(xml, settings);
    }
}
