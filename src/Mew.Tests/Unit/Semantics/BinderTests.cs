namespace Mew.Tests.Semantics;

public sealed partial class BinderTests
{
    [Fact]
    public void Should_Bind_Program()
    {
        // Given
        var text = @"
                fn foo() -> int {
                    let corgi = 3;
                    return corgi - 1;
                }

                let bar = 0;
                let baz = foo();
                let qux = baz + 1;
            ";

        var syntaxTrees = ImmutableArray.Create(
            ParserFixture.Parse(text));

        // When
        var result = CodeAnalysis.Semantics.SemanticModel.Create("test", syntaxTrees);

        // Then
        result.Functions.Count.ShouldBe(1);
        result.Statements.Length.ShouldBe(3);
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Assigning_Undeclared_Variable()
    {
        // Given
        var text = @"
                fn foo() {
                    corgi = 3;
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Undeclared variable 'corgi'");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Assigning_To_Undeclared_Variable()
    {
        // Given
        var text = @"
                fn foo() {
                    let corgi = waldo;
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Undeclared variable 'waldo'");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Wrong_Symbol_Type()
    {
        // Given
        var text = @"
                fn foo() {
                }

                foo = 3;
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: 'foo' is not a variable");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Undeclared_Type()
    {
        // Given
        var text = @"
                fn foo(bar: baz) {
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Undeclared type 'baz'");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Redeclared_Variable()
    {
        // Given
        var text = @"
                let foo = 1;
                let foo = 32;
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Variable 'foo' has already been declared");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Redeclared_Function()
    {
        // Given
        var text = @"
                fn foo() {
                }

                fn foo() {
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Function 'foo' has already been declared");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Redeclared_Function_With_Same_Parameters()
    {
        // Given
        var text = @"
                fn foo(bar: int) {
                }

                fn foo(baz: int) {
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Function 'foo' has already been declared");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Undefined_Binary_Operator()
    {
        // Given
        var text = @"
                let foo = 1 + false;
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Binary operator '+' is not defined for types 'int' and 'bool'");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Undefined_Logical_Operator()
    {
        // Given
        var text = @"
                let foo = 1 and false;
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Logical operator 'and' is not defined for types 'int' and 'bool'");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Undefined_Function()
    {
        // Given
        var text = @"
                let foo = bar();
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Undefined function 'bar'");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Function_Call_To_Non_Function()
    {
        // Given
        var text = @"
                let bar = 0;
                let foo = bar();
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: 'bar' is not a function");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Function_Call_With_Unconvertable_Argument()
    {
        // Given
        var text = @"
                let foo = bar(32, ""hello"");

                fn bar(baz: int, qux: bool) {
                    // NOP
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Cannot convert type 'string' to 'bool'");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Unreachable_While_Loop_Body()
    {
        // Given
        var text = @"
                while(false) {
                    // Never reached
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Unreachable code detected");
    }

    [Fact]
    public void Should_Return_Diagnostics_When_Using_Break_Statement_Outside_Of_Loop()
    {
        // Given
        var text = @"
                break;
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: No enclosing loop out of which to break");
    }

    [Fact]
    public void Should_Return_Diagnostics_When_Using_Continue_Statement_Outside_Of_Loop()
    {
        // Given
        var text = @"
                continue;
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: No enclosing loop out of which to continue");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Unreachable_Then_Branch()
    {
        // Given
        var text = @"
                if(false) {
                    // Never reached
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Unreachable code detected");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Unreachable_Else_Branch()
    {
        // Given
        var text = @"
                if(true) {
                    // Reached
                } else {
                    // Never reached
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Unreachable code detected");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Undefined_Unary_Operator()
    {
        // Given
        var text = @"
                let foo = !1;
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Unary operator '!' is not defined for type 'int'");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Returning_Expression_Within_Void_Function()
    {
        // Given
        var text = @"
                fn foo() {
                    return 1;
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Enclosing function does not return a value");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Wrong_Return_Type()
    {
        // Given
        var text = @"
                fn foo() -> bool {
                    return 1;
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: Wrong return type. Expected 'bool' but was given 'int'");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Missing_Return_Type()
    {
        // Given
        var text = @"
                fn foo() -> bool {
                    return;
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: An expression of type 'bool' was expected");
    }

    [Fact]
    public void Should_Return_Diagnostics_For_Function_Call_With_No_Overload()
    {
        // Given
        var text = @"
                let foo = bar(32, true, false);

                fn bar(baz: int, qux: bool) {
                    // NOP
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.AssertDiagnostics("Error: No overload for function 'bar' takes 3 arguments");
    }

    [Fact]
    public void Should_Bind_Parameters_As_Variables_In_Function_Scope()
    {
        // Given
        var text = @"
                fn bar(value: int) -> bool {
                    return value > 2;
                }
            ";

        // When
        var compilation = CompilerFixture.GetSemanticModel(text);

        // Then
        compilation.Diagnostics.Count.ShouldBe(0);
    }
}
