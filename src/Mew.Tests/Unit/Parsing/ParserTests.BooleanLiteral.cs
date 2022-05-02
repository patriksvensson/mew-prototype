namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/BooleanLiteralExpression")]
    public sealed class BooleanLiteral
    {
        [Fact]
        [Expectation("BooleanLiteral", "True")]
        public Task Should_Parse_Boolean_True_Literal()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = true;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("BooleanLiteral", "False")]
        public Task Should_Parse_Boolean_False_Literal()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = false;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }
    }
}