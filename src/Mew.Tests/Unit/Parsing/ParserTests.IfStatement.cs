namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/IfStatement")]
    public sealed class If
    {
        [Fact]
        [Expectation("If")]
        public Task Should_Parse_If_Expression()
        {
            // Given, When
            var expr = ParserFixture.Parse("if foo == 1 { foo = 2; }");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Theory]
        [InlineData("if", "Expected a literal value, a parenthesized expression, or a function call at this location")]
        [InlineData("if foo == 1", "Expected a literal value, a parenthesized expression, or a function call at this location")]
        [InlineData("if foo == 1 {", "Expected '}' after block")]
        public void Should_Produce_Correct_Diagnostics(string input, string expected)
        {
            // Given, When
            var result = ParserFixture.Parse(input);

            // Then
            result.Diagnostics.Count.ShouldBe(1);
            result.Diagnostics[0].Message.ShouldBe(expected);

            result.Root.Statements.Length.ShouldBe(1);
            result.Root.Statements[0].ShouldBeOfType<IfStatement>();
        }
    }
}
