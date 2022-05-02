namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/FunctionDeclarationStatement")]
    public sealed class FunctionDeclarationStatement
    {
        [Fact]
        [Expectation("FunctionDeclaration")]
        public Task Should_Parse_Function_Call()
        {
            // Given, When
            var expr = ParserFixture.Parse("fn foo(bar: int, qux: bool)\n{\n\tlet corgi = 1;\n}");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("FunctionDeclarationWithReturnType")]
        public Task Should_Parse_Function_Call_With_Return_Type()
        {
            // Given, When
            var expr = ParserFixture.Parse("fn foo(bar: int, qux: bool) -> int\n{\n\tlet corgi = 1;\n}");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Theory]
        [InlineData("fn", "Expected function name")]
        [InlineData("fn foo", "Expected '(' after function name")]
        [InlineData("fn foo(", "Expected ')'")]
        [InlineData("fn foo(bar", "Expected ':'")]
        [InlineData("fn foo(bar:", "Expected type")]
        [InlineData("fn foo(bar:int", "Expected ')'")]
        [InlineData("fn foo()", "Expected '{'")]
        [InlineData("fn foo() {", "Expected '}' after block")]
        public void Should_Produce_Correct_Diagnostics(string input, string expected)
        {
            // Given, When
            var result = ParserFixture.Parse(input);

            // Then
            result.Diagnostics.Count.ShouldBe(1);
            result.Diagnostics[0].Message.ShouldBe(expected);
        }
    }
}