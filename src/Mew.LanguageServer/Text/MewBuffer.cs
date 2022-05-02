namespace Mew.LangServer.Text;

public sealed class MewBuffer
{
    public DocumentUri Uri { get; }
    public int? Version { get; }
    public Compilation Compilation { get; }
    public List<TextLine> Lines { get; }
    public int Length { get; }

    public MewBuffer(DocumentUri uri, int? version, string text, Compilation compilation)
    {
        Uri = uri ?? throw new ArgumentNullException(nameof(uri));
        Version = version;
        Compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
        Lines = TextCursor.Split(text);
        Length = Lines.Sum(x => x.Length + x.LineBreak.Length);
    }

    public OmniSharp.Extensions.LanguageServer.Protocol.Models.Range GetRange(TextSpan span)
    {
        var (startLine, startColumn) = GetPositionFromOffset(span.Position);
        var (endLine, endColumn) = GetPositionFromOffset(span.Position + span.Length);

        return new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range
        {
            Start = new Position(startLine, startColumn),
            End = new Position(endLine, endColumn),
        };
    }

    public int? GetOffsetFromPosition(Position position)
    {
        if (position.Line > Lines.Count)
        {
            return null;
        }

        var line = Lines[position.Line];
        return line.Offset + position.Character;
    }

    public (int LineIndex, int ColumnIndex) GetPositionFromOffset(int offset)
    {
        if (offset < 0)
        {
            throw new InvalidOperationException("Offset must be equal or greater than zero (0)");
        }

        if (offset > Length)
        {
            throw new InvalidOperationException("Offset exceeded the source length");
        }

        var lineIndex = GetLineIndex(offset);
        var line = Lines[lineIndex];
        var columnIndex = offset - line.Offset;

        if (columnIndex > Length)
        {
            columnIndex = Math.Max(0, Length);
        }

        return (lineIndex, columnIndex);
    }

    private int GetLineIndex(int offset)
    {
        if (offset < 0)
        {
            throw new InvalidOperationException("Offset must be equal or greater than zero (0)");
        }

        var index = 0;
        foreach (var line in Lines)
        {
            if (offset >= line.Offset && offset <= line.Offset + line.Length)
            {
                return index;
            }

            index++;
        }

        // Grab the last line
        return Math.Max(0, index - 1);
    }
}
