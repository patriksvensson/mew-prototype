namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/FunctionCallExpression")]
    public sealed class FunctionCallExpression
    {
        [Fact]
        [Expectation("FunctionCall")]
        public Task Should_Parse_Function_Call()
        {
            // Given, When
            var expr = ParserFixture.Parse("foo(bar);");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Theory]
        [InlineData("foo(", "Expected a literal value, a parenthesized expression, or a function call at this location")]
        [InlineData("foo(bar", "Expected ')'")]
        [InlineData("foo(bar)", "Expected ';'")]
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