namespace Mew.LangServer;

public sealed class MewLogger
{
    private readonly string? _origin;
    private readonly ILanguageServerFacade _facade;

    public MewLogger(ILanguageServerFacade facade)
    {
        _facade = facade ?? throw new ArgumentNullException(nameof(facade));
    }

    private MewLogger(string origin, ILanguageServerFacade facade)
    {
        _origin = origin ?? throw new ArgumentNullException(nameof(origin));
        _facade = facade ?? throw new ArgumentNullException(nameof(facade));
    }

    public MewLogger ForOrigin(string origin)
    {
        return new MewLogger(origin, _facade);
    }

    public void LogInfo(string message)
    {
        message = string.IsNullOrWhiteSpace(_origin) ? message : $"[{_origin}] {message}";
        _facade.Window.LogInfo(message);
    }

    public void LogError(string message)
    {
        message = string.IsNullOrWhiteSpace(_origin) ? message : $"[{_origin}] {message}";
        _facade.Window.LogError(message);
    }

    public void LogError(Exception ex, string? message = null)
    {
        message ??= "An error occured";

        _facade.Window.LogError("-----------------------------");

        if (_origin != null)
        {
            _facade.Window.LogError($"Origin: {_origin}");
        }

        _facade.Window.LogError($"{message}: {ex.Message}");
        _facade.Window.LogError($"{ex.StackTrace}");
        _facade.Window.LogError("-----------------------------");
    }
}
