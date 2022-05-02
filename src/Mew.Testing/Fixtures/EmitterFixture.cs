using Mew.CodeAnalysis.Emit;
using Mono.Cecil;

namespace Mew.Testing;

public sealed class EmitterFixture : IDisposable
{
    private readonly MemoryStream _stream;
    private readonly AssemblyDefinition _assembly;

    public EmitterFixture(string source)
    {
        // Parse
        var unit = CompilerFixture.GetSemanticModel(source);

        // Emit
        _stream = new MemoryStream();
        var emitter = new ILEmitter(unit, "foo");
        emitter.Emit(_stream);
        _stream.Seek(0, SeekOrigin.Begin);

        _assembly = AssemblyDefinition.ReadAssembly(_stream, new ReaderParameters
        {
            InMemory = true,
        });

        // Ensure assembly was read
        _assembly.ShouldNotBeNull("Could not read emitted assembly");
    }

    public void Dispose()
    {
        _assembly.Dispose();
    }

    public string FormatMethod(string typeName, string methodName)
    {
        var type = _assembly.MainModule.Types.FirstOrDefault(x => x.Name.Equals(typeName, StringComparison.Ordinal));
        type.ShouldNotBeNull($"Could not resolve method '{typeName}'");

        var method = type.Methods.FirstOrDefault(x => x.Name.Equals(methodName, StringComparison.Ordinal));
        method.ShouldNotBeNull($"Could not resolve method '{methodName}'");

        return CecilFormatter.FormatMethodBody(method);
    }
}
