namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/BlockStatement")]
    public sealed class Block
    {
        [Fact]
        [Expectation("Block")]
        public Task Should_Parse_Block()
        {
            // Given, When
            var expr = ParserFixture.Parse("{ let foo = bar; baz = qux; }");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Fact]
        [Expectation("BlockWithErrors")]
        public Task Should_Parse_Block_Regardless_Of_Errors()
        {
            // Given, When
            var expr = ParserFixture.Parse("{\n\tfoo\n\tlet foo = 0;\n}");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }

        [Theory]
        [InlineData("{ let foo = bar", "Expected ';'")]
        [InlineData("{ let foo = bar;", "Expected '}' after block")]
        public void Should_Produce_Correct_Diagnostics(string input, string expected)
        {
            // Given, When
            var result = ParserFixture.Parse(input);

            // Then
            result.Diagnostics.Count.ShouldBe(1);
            result.Diagnostics[0].Message.ShouldBe(expected);
        }
    }
}
