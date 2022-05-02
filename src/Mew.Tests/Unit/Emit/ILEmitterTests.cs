namespace Mew.Tests.Emit;

[UsesVerify]
[ExpectationPath("Emitter")]
public sealed class ILEmitterTests
{
    [Fact]
    [Expectation("FunctionCall")]
    public async Task FunctionCall()
    {
        // Given
        var source = @"
                let foo = bar(32, ""hello"");
                fn bar(baz: int, qux: string) -> bool {
                    return true;
                }
            ";

        // When
        var method = new EmitterFixture(source)
            .FormatMethod("Program", "Main");

        // Then
        await Verify(method);
    }

    [Fact]
    [Expectation("ExternFunctionCall")]
    public async Task ExternFunctionCall()
    {
        // Given
        var source = @"
                extern fn bar(number: int, text: string) -> bool;
                let foo = bar(32, ""hello"");
            ";

        // When
        var method = new EmitterFixture(source)
            .FormatMethod("Program", "Main");

        // Then
        await Verify(method);
    }

    [Fact]
    [Expectation("VariableDeclaration")]
    public async Task VariableDeclaration()
    {
        // Given
        var source = @"
                let foo = ""Hello World!"";
            ";

        // When
        var method = new EmitterFixture(source)
            .FormatMethod("Program", "Main");

        // Then
        await Verify(method);
    }

    [Fact]
    [Expectation("Assignment")]
    public async Task Assignment()
    {
        // Given
        var source = @"
                let foo = ""Hello World!"";
                foo = ""Bar"";
            ";

        // When
        var method = new EmitterFixture(source)
            .FormatMethod("Program", "Main");

        // Then
        await Verify(method);
    }
}