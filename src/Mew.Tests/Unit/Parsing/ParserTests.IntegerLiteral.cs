namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/IntegerLiteralExpression")]
    public sealed class IntegerLiteral
    {
        [Fact]
        [Expectation("IntegerLiteral")]
        public Task Should_Parse_Integer_Literal()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 32;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }
    }
}