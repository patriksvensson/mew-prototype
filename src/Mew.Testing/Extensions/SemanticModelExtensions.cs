using Mew.CodeAnalysis.Semantics;

namespace Mew.Testing;

public static class SemanticModelExtensions
{
    public static void AssertDiagnostics(this SemanticModel semanticModel, params string[] expectedMessages)
    {
        semanticModel.ShouldNotBeNull("No semantic model provided");

        expectedMessages.ShouldNotBeNull("No diagnostics provided");
        expectedMessages.Length.ShouldBeGreaterThan(0, "No diagnostics provided");

        semanticModel.Diagnostics.Count.ShouldBe(expectedMessages.Length);

        var matched = 0;
        foreach (var expected in expectedMessages)
        {
            var wasMatched = false;
            foreach (var diagnostic in semanticModel.Diagnostics)
            {
                var formatted = FormatDiagnostic(diagnostic);
                if (formatted.Equals(expected, StringComparison.Ordinal))
                {
                    wasMatched = true;
                    matched++;
                    break;
                }
            }

            if (!wasMatched)
            {
                var all = string.Join("\n", semanticModel.Diagnostics.Select(d => "  -> " + FormatDiagnostic(d)));
                throw new InvalidOperationException(
                    $"\nDiagnostic missing:\n  -> {expected}\nFound:\n{all}");
            }
        }
    }

    private static string FormatDiagnostic(Diagnostic diagnostic)
    {
        return $"{diagnostic.Severity}: {diagnostic.Message}";
    }
}
