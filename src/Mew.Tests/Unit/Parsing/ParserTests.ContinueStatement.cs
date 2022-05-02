namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/ContinueStatement")]
    public sealed class Continue
    {
        [Fact]
        [Expectation("Continue")]
        public Task Should_Parse_Continue()
        {
            // Given, When
            var expr = ParserFixture.Parse("loop { continue; }");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Theory]
        [InlineData("continue", "Expected ';'")]
        public void Should_Produce_Correct_Diagnostics(string input, string expected)
        {
            // Given, When
            var result = ParserFixture.Parse(input);

            // Then
            result.Diagnostics.Count.ShouldBe(1);
            result.Diagnostics[0].Message.ShouldBe(expected);

            result.Root.Statements.Length.ShouldBe(1);
            result.Root.Statements[0].ShouldBeOfType<ContinueStatement>();
        }
    }
}
