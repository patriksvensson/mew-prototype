namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/UnaryExpression")]
    public sealed class UnaryExpression
    {
        [Fact]
        [Expectation("Negation")]
        public Task Should_Parse_Negation()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = !true;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }
    }
}
