namespace Mew.Commands;

internal sealed class CompileCommand : Command<CompileCommand.Settings>
{
    private readonly IAnsiConsole _console;
    private readonly CompilerFeature _compiler;
    private readonly ParserFeature _parser;
    private readonly DiagnosticRenderer _renderer;
    private readonly ILogger _logger;

    public sealed class Settings : CommandSettings
    {
        [TypeConverter(typeof(FilePathConverter))]
        [CommandArgument(0, "<FILENAME>")]
        public FilePath Filename { get; set; }

        [TypeConverter(typeof(DirectoryPathConverter))]
        [CommandOption("-o|--output <OUTPUT>")]
        public DirectoryPath? Output { get; set; }

        [CommandOption("-m|--module <NAME>")]
        public string? Name { get; set; }

        [CommandOption("--llvm")]
        public bool Llvm { get; set; }

        [CommandOption("--dump")]
        public bool Dump { get; set; }

        public Settings(FilePath filename, DirectoryPath? output, string? name, bool llvm, bool dump)
        {
            Filename = filename ?? throw new ArgumentNullException(nameof(filename));
            Output = output;
            Name = name;
            Llvm = llvm;
            Dump = dump;
        }
    }

    public CompileCommand(
        IAnsiConsole console, CompilerFeature compiler,
        ParserFeature parser, DiagnosticRenderer renderer,
        ILogger logger)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var input = settings.Filename;
        var output = settings.Output ?? new DirectoryPath(".");

        // Parse semantic model
        var model = _parser.Parse(input);
        if (model.Diagnostics.HasErrors)
        {
            _renderer.Render(model);
            return -1;
        }

        // Compile
        _compiler.Compile(model, new CompilerFeatureSettings(output)
        {
            Name = settings.Name,
            Llvm = settings.Llvm,
            Dump = settings.Dump,
        });

        return 0;
    }
}
