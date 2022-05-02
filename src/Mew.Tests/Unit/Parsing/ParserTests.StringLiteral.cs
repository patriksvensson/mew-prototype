namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/StringLiteral")]
    public sealed class StringLiteral
    {
        [Fact]
        [Expectation("StringLiteral")]
        public Task Should_Parse_String_Literal()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = \"32\";");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }
    }
}