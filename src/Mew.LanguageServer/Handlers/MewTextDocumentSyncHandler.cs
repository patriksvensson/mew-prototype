namespace Mew.LangServer.Handlers;

public sealed class MewTextDocumentSyncHandler : TextDocumentSyncHandlerBase
{
    private readonly MewBufferManager _bufferManager;
    private readonly MewDiagnosticPublisher _diagnosticPublisher;
    private readonly MewLogger _logger;
    private readonly DocumentSelector _documentSelector;

    public MewTextDocumentSyncHandler(
        MewBufferManager bufferManager,
        MewDiagnosticPublisher diagnosticPublisher,
        MewLogger logger)
    {
        _bufferManager = bufferManager ?? throw new ArgumentNullException(nameof(bufferManager));
        _diagnosticPublisher = diagnosticPublisher ?? throw new ArgumentNullException(nameof(diagnosticPublisher));
        _logger = logger?.ForOrigin("MewTextDocumentSyncHandler") ?? throw new ArgumentNullException(nameof(logger));
        _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.mew",
            });
    }

    public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
    {
        return new TextDocumentAttributes(uri, "mew");
    }

    protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(
        SynchronizationCapability capability,
        ClientCapabilities clientCapabilities)
    {
        return new TextDocumentSyncRegistrationOptions()
        {
            DocumentSelector = _documentSelector,
            Change = TextDocumentSyncKind.Full,
        };
    }

    public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
    {
        _logger.LogInfo($"Opening document: {request.TextDocument.Uri}");

        UpdateBuffer(request.TextDocument.Uri, request.TextDocument.Text, request.TextDocument.Version);

        return Unit.Task;
    }

    public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
    {
        var text = request.ContentChanges.FirstOrDefault()?.Text ?? string.Empty;
        UpdateBuffer(request.TextDocument.Uri, text, request.TextDocument.Version);

        return Unit.Task;
    }

    public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
    {
        return Unit.Task;
    }

    public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
    {
        _logger.LogInfo($"Closing document: {request.TextDocument.Uri}");

        if (_bufferManager.RemoveBuffer(request.TextDocument.Uri, out var version))
        {
            _diagnosticPublisher.ClearDiagnostics(request.TextDocument.Uri, version);
        }

        return Unit.Task;
    }

    private void UpdateBuffer(DocumentUri uri, string text, int? version)
    {
        try
        {
            var documentPath = uri.ToString();

            // Parse the document
            var compilation = Compilation.Create("lsp_temp")
                .AddSyntaxTree(SyntaxTree.Parse(documentPath, text));

            // Update the buffer
            var buffer = new MewBuffer(uri, version, text, compilation);
            _bufferManager.UpdateBuffer(documentPath, buffer);

            var diagnostics = compilation.GetDiagnostics();
            if (diagnostics.HasErrors)
            {
                _diagnosticPublisher.PublishDiagnostics(diagnostics);
            }
            else
            {
                _diagnosticPublisher.ClearDiagnostics(uri, version);
            }

            _logger.LogInfo($"Updated buffer for document: {documentPath} ({version})");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex);
        }
    }
}