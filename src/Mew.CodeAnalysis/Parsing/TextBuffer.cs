namespace Mew.CodeAnalysis;

internal sealed class TextBuffer
{
    private readonly ReadOnlyMemory<char> _buffer;
    private int _position;

    public bool CanRead => _position < _buffer.Length;
    public int Position => _position;
    public char Current => _buffer.Span[Position];

    public TextBuffer(string content)
    {
        _buffer = content.AsMemory();
        _position = 0;
    }

    public char Peek()
    {
        return Peek(0);
    }

    public char Peek(int offset)
    {
        if (offset < 0)
        {
            throw new InvalidOperationException("Offset must be greater than or equal to 0");
        }

        var pos = _position + offset;
        if (pos >= _buffer.Length)
        {
            return '\0';
        }

        return _buffer.Span[pos];
    }

    public void Move(int position)
    {
        _position = position;
    }

    public char Read()
    {
        var result = Peek();
        if (result != '\0')
        {
            _position++;
        }

        return result;
    }

    public ReadOnlyMemory<char> Slice(int start, int stop)
    {
        return _buffer[start..stop];
    }
}
