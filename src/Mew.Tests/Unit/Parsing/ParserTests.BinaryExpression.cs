namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/BinaryExpression")]
    public sealed class BinaryExpression
    {
        [Fact]
        [Expectation("Equality")]
        public Task Should_Parse_Equality()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 == 2;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("NonEquality")]
        public Task Should_Parse_Non_Equality()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 != 2;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("GreaterThan")]
        public Task Should_Parse_Greater_Than()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 > 2;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("GreaterThanOrEqualTo")]
        public Task Should_Parse_Greater_Than_Or_Equal_To()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 >= 2;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("LessThan")]
        public Task Should_Parse_Less_Than()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 < 2;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("LessThanOrEqualTo")]
        public Task Should_Parse_Less_Than_Or_Equal_To()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 <= 2;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("Addition")]
        public Task Should_Parse_Addition()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 + 22;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("Subtraction")]
        public Task Should_Parse_Subtraction()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 - 22;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("Multiplication")]
        public Task Should_Parse_Multiplication()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 * 22;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("Division")]
        public Task Should_Parse_Division()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 / 22;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("Modolu")]
        public Task Should_Parse_Modolu()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = 1 % 22;");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }
    }
}
