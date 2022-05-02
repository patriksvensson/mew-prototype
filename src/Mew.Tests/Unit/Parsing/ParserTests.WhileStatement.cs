namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/WhileStatement")]
    public sealed class While
    {
        [Fact]
        [Expectation("While")]
        public Task Should_Parse_While()
        {
            // Given, When
            var expr = ParserFixture.Parse("while foo > 2\n{\n\tfoo = 2;\n}");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Theory]
        [InlineData("while", "Expected a literal value, a parenthesized expression, or a function call at this location")]
        [InlineData("while foo", "Expected '{'")]
        [InlineData("while foo {", "Expected '}' after block")]
        public void Should_Produce_Correct_Diagnostics(string input, string expected)
        {
            // Given, When
            var result = ParserFixture.Parse(input);

            // Then
            result.Diagnostics.Count.ShouldBe(1);
            result.Diagnostics[0].Message.ShouldBe(expected);

            result.Root.Statements.Length.ShouldBe(1);
            result.Root.Statements[0].ShouldBeOfType<WhileStatement>();
        }
    }
}
