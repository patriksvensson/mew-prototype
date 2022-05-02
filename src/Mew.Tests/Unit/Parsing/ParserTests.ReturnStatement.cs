namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/ReturnStatement")]
    public sealed class Return
    {
        [Fact]
        [Expectation("Return")]
        public Task Should_Parse_Return()
        {
            // Given, When
            var expr = ParserFixture.Parse("return;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("ReturnWithExpression")]
        public Task Should_Parse_Return_With_Expression()
        {
            // Given, When
            var expr = ParserFixture.Parse("return foo == bar;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Theory]
        [InlineData("return", "Expected a literal value, a parenthesized expression, or a function call at this location")]
        [InlineData("return foo", "Expected ';'")]
        public void Should_Produce_Correct_Diagnostics(string input, string expected)
        {
            // Given, When
            var result = ParserFixture.Parse(input);

            // Then
            result.Diagnostics.Count.ShouldBe(1);
            result.Diagnostics[0].Message.ShouldBe(expected);

            result.Root.Statements.Length.ShouldBe(1);
            result.Root.Statements[0].ShouldBeOfType<ReturnStatement>();
        }
    }
}
