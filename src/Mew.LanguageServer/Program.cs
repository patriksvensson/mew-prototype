namespace Mew.LangServer;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var app = new CommandApp<StartCommand>();
        return await app.RunAsync(args);
    }
}

public sealed class StartCommand : AsyncCommand<StartCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("--debug")]
        public bool Debug { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (settings.Debug)
        {
            Debugger.Launch();
        }

        var server = await LanguageServer.From(options =>
            options
                .WithInput(Console.OpenStandardInput())
                .WithOutput(Console.OpenStandardOutput())
                .WithLoggerFactory(new LoggerFactory())
                .AddDefaultLoggingProvider()
                .WithServices(ConfigureServices)
                .WithHandler<MewTextDocumentSyncHandler>()
                .WithHandler<MewSemanticTokensHandler>()
                .WithHandler<MewHoverHandler>());

        await server.WaitForExit;
        return 0;
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<MewBufferManager>();
        services.AddSingleton<MewLogger>();
        services.AddSingleton<MewDiagnosticPublisher>();
        services.AddSingleton<MewOutlinePublisher>();
    }
}
