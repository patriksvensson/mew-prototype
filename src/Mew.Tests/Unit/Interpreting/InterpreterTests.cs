using Mew.CodeAnalysis.Interpreting;

namespace Mew.Tests;

public sealed class InterpreterTests
{
    [Fact]
    public void Should_Interpret_Variable_Declaration()
    {
        // Given
        var model = CompilerFixture.GetSemanticModel("let foo = 2;");

        // When
        var interpreter = new Interpreter();
        var result = interpreter.Evaluate(model);

        // Then
        result.ShouldBeNull();
    }

    [Fact]
    public void Should_Interpret_External_Function_Call()
    {
        // Given
        var invoker = new FakeExternalFunctionInvoker();
        var interpreter = new Interpreter(invoker);
        var model = CompilerFixture.GetSemanticModel(
            @"
                extern fn print();
                print();
            ");

        // When
        interpreter.Evaluate(model);

        // Then
        invoker.ReceivedCall("print").ShouldBeTrue();
    }

    [Fact]
    public void Should_Interpret_Function_Call()
    {
        // Given
        var invoker = new FakeExternalFunctionInvoker();
        var interpreter = new Interpreter(invoker);
        var model = CompilerFixture.GetSemanticModel(
            @"
                extern fn print_external();
                fn print() {
                    print_external();
                }

                print();
            ");

        // When
        interpreter.Evaluate(model);

        // Then
        invoker.ReceivedCall("print_external").ShouldBeTrue();
    }

    [Fact]
    public void Should_Interpret_If_Statement()
    {
        // Given
        var invoker = new FakeExternalFunctionInvoker();
        var interpreter = new Interpreter(invoker);
        var model = CompilerFixture.GetSemanticModel(
            @"
                let corgi = 3;
                if corgi > 2 {
                    return 3;
                }
                return 0;
            ");

        // When
        var result = interpreter.Evaluate(model);

        // Then
        result.ShouldBe(3);
    }

    [Fact]
    public void Should_Interpret_Assignment_Statement()
    {
        // Given
        var invoker = new FakeExternalFunctionInvoker();
        var interpreter = new Interpreter(invoker);
        var model = CompilerFixture.GetSemanticModel(
            @"
                fn foo() -> int {
                    let bar = 0;
                    bar = 2;
                    return bar;
                }

                let corgi = 3;
                corgi = foo();
                return corgi;
            ");

        // When
        var result = interpreter.Evaluate(model);

        // Then
        result.ShouldBe(2);
    }
}
