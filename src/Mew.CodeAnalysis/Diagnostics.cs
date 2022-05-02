namespace Mew.CodeAnalysis;

public sealed class Diagnostics : List<Diagnostic>
{
    public bool HasErrors => this.Any(x => x.Severity == Severity.Error);

    public Diagnostics()
    {
    }

    public void Add(Location location, DiagnosticDescriptor descriptor)
    {
        Add(new Diagnostic(
            descriptor.Code,
            location,
            descriptor.Severity,
            descriptor.Message));
    }

    public Diagnostics(IEnumerable<Diagnostic> diagnostics)
        : base(diagnostics)
    {
    }

    public static Diagnostics Create(IEnumerable<SyntaxTree> syntaxTrees)
    {
        return syntaxTrees
            .Select(t => t.Diagnostics)
            .Aggregate((a, b) => a.Merge(b))
                ?? new Diagnostics();
    }

    public Diagnostics Merge(Diagnostics other)
    {
        if (other == null)
        {
            return this;
        }

        return new Diagnostics(this.Concat(other));
    }
}
