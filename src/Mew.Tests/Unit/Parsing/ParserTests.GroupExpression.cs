namespace Mew.Tests.Parsing;

public sealed partial class ParserTests
{
    [UsesVerify]
    [ExpectationPath("Parser/GroupExpression")]
    public sealed class GroupExpression
    {
        [Fact]
        [Expectation("Group")]
        public Task Should_Parse_Group_Expression()
        {
            // Given, When
            var expr = ParserFixture.Parse("let x = (32);");

            // Then
            return VerifierEx.VerifySyntaxTree(expr);
        }
    }
}