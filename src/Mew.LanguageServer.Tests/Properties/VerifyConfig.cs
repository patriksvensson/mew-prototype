namespace Mew.LanguageServer.Tests.Properties;

public static class VerifyConfig
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.DerivePathInfo(Expectations.Initialize);
    }
}