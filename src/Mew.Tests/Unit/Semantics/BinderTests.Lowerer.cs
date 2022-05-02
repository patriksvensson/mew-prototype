namespace Mew.Tests.Semantics;

public sealed partial class LowererTests
{
    [UsesVerify]
    [ExpectationPath("Lowerer")]
    public sealed class Output
    {
        [Fact]
        [Expectation("IfStatement")]
        public async Task Should_Lower_If_Statements()
        {
            // Given
            var text = @"
                extern fn print(text: string);

                let corgi = 3;
                if corgi == 3 {
                    print(""Three"");
                } else {
                    print(""Not three!"");
                }
            ";

            // When
            var model = CompilerFixture.GetSemanticModel(text);

            // Then
            await VerifierEx.VerifySemanticModel(model);
        }

        [Fact]
        [Expectation("WhileStatement")]
        public async Task Should_Lower_While_Statements()
        {
            // Given
            var text = @"
                extern fn print(text: int);

                let corgi = 0;
                while(corgi != 3) {
                    corgi = corgi + 1;
                }
            ";

            // When
            var model = CompilerFixture.GetSemanticModel(text);

            // Then
            await VerifierEx.VerifySemanticModel(model);
        }
    }
}
