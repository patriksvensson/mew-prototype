namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/ExternalFunctionDeclarationStatement")]
    public sealed class ExternalFunctionDeclarationStatement
    {
        [Fact]
        [Expectation("ExternalFunctionDeclaration")]
        public Task Should_Parse_Function_Call()
        {
            // Given, When
            var expr = ParserFixture.Parse("extern fn foo(bar: int, qux: bool);");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("ExternalFunctionDeclarationWithReturnType")]
        public Task Should_Parse_Function_Call_With_Return_Type()
        {
            // Given, When
            var expr = ParserFixture.Parse("extern fn foo(bar: int, qux: bool) -> string;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Theory]
        [InlineData("extern", "Expected 'fn' keyword")]
        [InlineData("extern fn", "Expected function name")]
        [InlineData("extern fn foo", "Expected '(' after function name")]
        [InlineData("extern fn foo(", "Expected ')'")]
        [InlineData("extern fn foo(bar", "Expected ':'")]
        [InlineData("extern fn foo(bar:", "Expected type")]
        [InlineData("extern fn foo(bar:int", "Expected ')'")]
        [InlineData("extern fn foo()", "Expected ';'")]
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