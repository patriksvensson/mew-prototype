namespace Mew.Infrastructure;

internal interface ILogger
{
    void Info(string markup);
    void Error(string markup);
}

internal sealed class ConsoleLogger : ILogger
{
    private readonly IAnsiConsole _console;

    public ConsoleLogger(IAnsiConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public void Info(string markup)
    {
        if (string.IsNullOrWhiteSpace(markup))
        {
            _console.WriteLine();
        }
        else
        {
            _console.MarkupLine(markup);
        }
    }

    public void Error(string markup)
    {
        if (!string.IsNullOrWhiteSpace(markup))
        {
            _console.MarkupLine($"[red]Error:[/] {markup}");
        }
    }
}
