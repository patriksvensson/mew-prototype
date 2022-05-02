namespace Mew;

public static class Program
{
    public static int Main(string[] args)
    {
        var container = BuildContainer();

        var app = new CommandApp<CompileCommand>(container);
        app.Configure(config =>
        {
            config.SetApplicationName("mew");
            config.UseStrictParsing();
        });

        return app.Run(args);
    }

    private static TypeRegistrar BuildContainer()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ParserFeature>();
        services.AddSingleton<CompilerFeature>();
        services.AddSingleton<DiagnosticRenderer>();
        services.AddSingleton<ILogger, ConsoleLogger>();

        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<IEnvironment, Spectre.IO.Environment>();

        return new TypeRegistrar(services);
    }
}
