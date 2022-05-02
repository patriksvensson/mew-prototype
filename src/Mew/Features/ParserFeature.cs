namespace Mew.Features;

internal sealed class ParserFeature
{
    private readonly IFileSystem _fileSystem;
    private readonly IEnvironment _environment;
    private readonly ILogger _logger;

    public ParserFeature(IFileSystem fileSystem, IEnvironment environment, ILogger logger)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public SemanticModel Parse(FilePath input)
    {
        input = input.MakeAbsolute(_environment);

        using (var stream = _fileSystem.File.OpenRead(input))
        using (var reader = new StreamReader(stream))
        {
            var source = reader.ReadToEnd();
            var syntaxTree = SyntaxTree.Parse(input.FullPath, source);

            var name = input.GetFilenameWithoutExtension().FullPath;
            var compilation = Compilation.Create(name).AddSyntaxTree(syntaxTree);

            return compilation.GetSemanticModel();
        }
    }
}
