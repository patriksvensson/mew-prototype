namespace Mew.LangServer.Outline;

public sealed class MewOutlinePublisher
{
    private readonly ILanguageServerFacade _router;
    private readonly MewLogger _logger;

    public MewOutlinePublisher(
        ILanguageServerFacade router,
        MewLogger logger)
    {
        _router = router ?? throw new ArgumentNullException(nameof(router));
        _logger = logger?.ForOrigin("MewOutlinePublisher") ?? throw new ArgumentNullException(nameof(logger));
    }

    public void PublishOutline(MewBuffer buffer)
    {
        try
        {
            var outline = MewOutlineBuilder.Build(buffer);
            _router.SendNotification("custom/updated", outline);
            _logger.LogInfo($"Sent outline notification: {buffer.Uri} ({buffer.Version})");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not send outline notification");
        }
    }
}
