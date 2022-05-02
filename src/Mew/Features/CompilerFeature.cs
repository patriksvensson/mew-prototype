using System.Runtime.InteropServices;

namespace Mew.Features;

public sealed class CompilerFeatureSettings
{
    public DirectoryPath Output { get; }
    public string? Name { get; set; }
    public bool Llvm { get; set; }
    public bool Dump { get; set; }

    public CompilerFeatureSettings(DirectoryPath output)
    {
        Output = output ?? throw new ArgumentNullException(nameof(output));
    }
}

public sealed class CompilerFeatureResult
{
    public FilePath Module { get; }
    public FilePath? Configuration { get; }

    public CompilerFeatureResult(FilePath module, FilePath? configuration)
    {
        Module = module;
        Configuration = configuration;
    }
}

internal sealed class CompilerFeature
{
    private readonly IFileSystem _fileSystem;
    private readonly IEnvironment _environment;
    private readonly ILogger _logger;

    public CompilerFeature(IFileSystem fileSystem, IEnvironment environment, ILogger logger)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public CompilerFeatureResult Compile(SemanticModel model, CompilerFeatureSettings settings)
    {
        if (settings.Llvm)
        {
            var output = settings.Output.CombineWithFilePath(settings.Name ?? model.Name);
            output = output.AppendExtension(".bc").MakeAbsolute(_environment);

            var emitter = new LLVMEmitter(_logger.Info);
            emitter.Emit(model, output.FullPath, settings.Dump);

            return new CompilerFeatureResult(output, null);
        }
        else
        {
            return EmitCIL(model, settings);
        }
    }

    private CompilerFeatureResult EmitCIL(SemanticModel model, CompilerFeatureSettings settings)
    {
        // Get the output file name.
        var output = settings.Output.CombineWithFilePath(settings.Name ?? model.Name);
        output = output.AppendExtension(GetOutputExtension()).MakeAbsolute(_environment);

        // Generate the IL for the semantic model
        EmitIL(model, output);

        // Emit the runtime config needed by 'dotnet exec'
        var config = EmitRunTimeConfig(output, "6.0.4");

        // Make sure we can find the runtime library
        _logger.Info("Resolving Mew runtime library...");
        var runtimeLibrary = GetRuntimeLibrary();
        if (runtimeLibrary.Exists)
        {
            // Copy the runtime library
            CopyRuntimeLibrary(runtimeLibrary, output.GetDirectory());
        }
        else
        {
            _logger.Error("Could not resolve Mew runtime library.");
        }

        // Return the result
        return new CompilerFeatureResult(output, config);
    }

    private IFile GetRuntimeLibrary()
    {
        return _fileSystem.GetFile(
            new FilePath(typeof(CompilerFeature).Assembly.Location)
                .GetDirectory()
                .CombineWithFilePath("mewstd.dll")
                .MakeAbsolute(_environment));
    }

    private void EmitIL(SemanticModel model, FilePath output)
    {
        _logger.Info($"Emitting: [yellow]{output}[/]");

        var name = output.GetFilenameWithoutExtension().FullPath;
        using (var outputStream = _fileSystem.File.OpenWrite(output))
        {
            var emitter = new ILEmitter(model, name);
            emitter.Emit(outputStream);
        }
    }

    private FilePath EmitRunTimeConfig(FilePath output, string version)
    {
        var path = output.GetDirectory()
            .CombineWithFilePath(
                output.GetFilenameWithoutExtension()
                    .AppendExtension(".runtimeconfig.json"));

        _logger.Info($"Writing runtime config: [yellow]{path}[/]");

        using (var stream = _fileSystem.File.OpenWrite(path))
        using (var writer = new StreamWriter(stream))
        {
            writer.Write(
                "{\n\t\"runtimeOptions\": {\n\t\t\"tfm\": \"net6.0\",\n\t\t\"framework\": " +
                $"{{\n\t\t\t\"name\": \"Microsoft.NETCore.App\",\n\t\t\t\"version\": \"{version}\"\n\t\t}}\n\t}}\n}}\n");
        }

        return path;
    }

    private FilePath CopyRuntimeLibrary(IFile file, DirectoryPath destination)
    {
        var path = destination.CombineWithFilePath("mewstd.dll");
        if (!_fileSystem.File.Exists(path))
        {
            _logger.Info($"Copying runtime library: [yellow]{path}[/]");
            file.Copy(path, true);
        }

        return path;
    }

    private static string GetOutputExtension()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "dll";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "dylib";
        }
        else
        {
            return "so";
        }
    }
}
