namespace Mew.LangServer;

public sealed class MewDiagnosticPublisher
{
    private readonly ILanguageServerFacade _facade;
    private readonly MewBufferManager _bufferManager;
    private readonly MewLogger _logger;

    public MewDiagnosticPublisher(
        ILanguageServerFacade facade,
        MewBufferManager bufferManager,
        MewLogger logger)
    {
        _facade = facade ?? throw new ArgumentNullException(nameof(facade));
        _bufferManager = bufferManager ?? throw new ArgumentNullException(nameof(bufferManager));
        _logger = logger?.ForOrigin("MewDiagnosticPublisher") ?? throw new ArgumentNullException(nameof(logger));
    }

    public void ClearDiagnostics(DocumentUri uri, int? version)
    {
        _facade.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
        {
            Uri = uri,
            Version = version,
            Diagnostics = new Container<OmnisharpDiagnostic>(),
        });
    }

    public void PublishDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        // Group diagnostics by their filename
        foreach (var group in diagnostics.GroupBy(x => x.Location.Path))
        {
            // If the schema is `unknown`, assume the diagnostics come from the currently opened file
            var groupUri = DocumentUri.From(group.Key);

            // Get the buffer for the diagnostic
            var buffer = _bufferManager.GetBuffer(groupUri);
            if (buffer != null)
            {
                // Publish diagnostics
                _facade.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
                {
                    Uri = buffer.Uri,
                    Version = buffer.Version,
                    Diagnostics = new Container<OmnisharpDiagnostic>(
                    group.Select(d => ToOmnisharpDiagnostic(d, buffer))),
                });

                _logger.LogInfo($"Published {group.Count()} diagnostic(s) for document: {groupUri}");
            }
            else
            {
                _logger.LogError(
                    $"Could not get buffer for '{groupUri}'. " +
                    $"{group.Count()} diagnostic(s) not sent");
            }
        }
    }

    private static OmnisharpDiagnostic ToOmnisharpDiagnostic(Diagnostic diagnostic, MewBuffer buffer)
    {
        return new OmnisharpDiagnostic
        {
            Code = diagnostic.Code,
            Severity = diagnostic.Severity == Severity.Error ? DiagnosticSeverity.Error : DiagnosticSeverity.Warning,
            Message = diagnostic.Message,
            Range = buffer.GetRange(diagnostic.Location.Span),
        };
    }
}
