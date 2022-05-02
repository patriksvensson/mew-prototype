namespace Mew.LangServer.Text;

public sealed class MewBufferManager
{
    private readonly ConcurrentDictionary<DocumentUri, MewBuffer> _buffers;
    private readonly MewOutlinePublisher _publisher;
    private readonly MewLogger _logger;

    public MewBufferManager(MewOutlinePublisher publisher, MewLogger logger)
    {
        _buffers = new ConcurrentDictionary<DocumentUri, MewBuffer>();
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void UpdateBuffer(DocumentUri documentPath, MewBuffer buffer)
    {
        _buffers.AddOrUpdate(documentPath, buffer, (_, _) => buffer);
        _publisher.PublishOutline(buffer);
    }

    public MewBuffer? GetBuffer(DocumentUri documentPath)
    {
        return _buffers.TryGetValue(documentPath, out var buffer) ? buffer : null;
    }

    public bool RemoveBuffer(DocumentUri documentPath, out int? version)
    {
        var result = _buffers.TryRemove(documentPath, out var buffer);
        version = buffer?.Version;

        if (result)
        {
            var versionString = version?.ToString() ?? "unknown";
            _logger.LogInfo($"Removed buffer for document: {documentPath} ({versionString})");
        }
        else
        {
            _logger.LogError($"Could not remove buffer: {documentPath}");
        }

        return result;
    }
}
