namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/AssignmentExpression")]
    public sealed class Assignments
    {
        [Fact]
        [Expectation("Assignment")]
        public Task Should_Parse_Assignment()
        {
            // Given, When
            var expr = ParserFixture.Parse("foo = 1;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Theory]
        [InlineData("foo =", "Expected a literal value, a parenthesized expression, or a function call at this location")]
        [InlineData("foo = 1", "Expected ';'")]
        public void Should_Produce_Correct_Diagnostics(string input, string expected)
        {
            // Given, When
            var result = ParserFixture.Parse(input);

            // Then
            result.Diagnostics.Count.ShouldBe(1);
            result.Diagnostics[0].Message.ShouldBe(expected);

            result.Root.Statements.Length.ShouldBe(1);
            result.Root.Statements[0].ShouldBeOfType<ExpressionStatement>().And(expr =>
            {
                expr.Expression.ShouldBeOfType<AssignmentExpression>();
            });
        }
    }
}
