namespace Mew.Features;

internal sealed class DiagnosticRenderer
{
    private readonly IAnsiConsole _console;

    public DiagnosticRenderer(IAnsiConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public void Render(SemanticModel model)
    {
        var repo = new InMemorySourceRepository();
        foreach (var tree in model.SyntaxTrees)
        {
            repo.Register(new FilePath(tree.Path).GetFilename().FullPath, tree.Source);
        }

        var report = new Report(repo);
        foreach (var group in model.Diagnostics.GroupBy(x => x.Code))
        {
            var item = report.AddDiagnostic(Errata.Diagnostic.Error(group.First().Message));
            item.WithCode(group.First().Code);

            foreach (var diagnostic in group)
            {
                item.WithLabel(
                    new Label(
                        new FilePath(diagnostic.Location.Path).GetFilename().FullPath,
                        new Errata.TextSpan(
                            diagnostic.Location.Span.Position,
                            diagnostic.Location.Span.Position + diagnostic.Location.Span.Length),
                        diagnostic.Message)
                    .WithPriority(1)
                    .WithColor(Color.Red));

                // Add notes with lower priority
                foreach (var note in diagnostic.Notes)
                {
                    item.WithLabel(
                        new Label(
                            new FilePath(note.Location.Path).GetFilename().FullPath,
                            new Errata.TextSpan(
                                note.Location.Span.Position,
                                note.Location.Span.Position + note.Location.Span.Length),
                            note.Message)
                        .WithPriority(2)
                        .WithColor(Color.Grey));
                }
            }
        }

        report.Render(_console);
    }
}
