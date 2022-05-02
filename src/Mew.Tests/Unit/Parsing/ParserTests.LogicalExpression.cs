namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/LogicalExpression")]
    public sealed class LogicalExpressions
    {
        [Fact]
        [Expectation("Or")]
        public Task Should_Parse_Logical_Or()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 || 2;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("And")]
        public Task Should_Parse_Logical_And()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 && 2;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("Or", "Keyword")]
        public Task Should_Parse_Logical_Or_Keyword()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 or 2;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("And", "Keyword")]
        public Task Should_Parse_Logical_And_Keyword()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 and 2;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }
    }
}
