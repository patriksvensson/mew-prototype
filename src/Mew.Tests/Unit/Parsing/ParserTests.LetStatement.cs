namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/LetStatement")]
    public sealed class Let
    {
        [Fact]
        [Expectation("Initializer")]
        public Task Should_Parse_With_Initializer()
        {
            // Given, When
            var expr = ParserFixture.Parse("let foo = 1;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Theory]
        [InlineData("let", "Expected variable name")]
        [InlineData("let foo", "Expected '='")]
        [InlineData("let foo =", "Expected a literal value, a parenthesized expression, or a function call at this location")]
        [InlineData("let foo = 0", "Expected ';'")]
        public void Should_Produce_Correct_Diagnostics(string input, string expected)
        {
            // Given, When
            var result = ParserFixture.Parse(input);

            // Then
            result.Diagnostics.Count.ShouldBe(1);
            result.Diagnostics[0].Message.ShouldBe(expected);

            result.Root.Statements.Length.ShouldBe(1);
            result.Root.Statements[0].ShouldBeOfType<LetStatement>();
        }
    }
}
