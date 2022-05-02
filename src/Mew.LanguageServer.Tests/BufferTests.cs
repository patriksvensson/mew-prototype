namespace Mew.LanguageServer.Tests;

public sealed class BufferTests
{
    [Fact]
    public void Should_Return_Correct_Line_Offset_For_CRLF()
    {
        // Given
        var text = "let foo = 32;\r\n\r\nlet\r\n\r\nlet foo = 32;\r\nlet foo = 32;\r\n";
        var compilation = CompilerFixture.GetCompilation(text);
        var buffer = new Buffer(DocumentUri.File("/test.mew"), 1, text, compilation);

        // When
        var range = buffer.GetRange(new TextSpan(20, 2));

        // Then
        range.Start.Line.ShouldBe(2);
        range.Start.Character.ShouldBe(3);
        range.End.Line.ShouldBe(3);
        range.End.Character.ShouldBe(0);
    }
}
