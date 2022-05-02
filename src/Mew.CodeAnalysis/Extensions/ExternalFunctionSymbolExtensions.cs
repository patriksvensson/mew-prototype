namespace Mew.CodeAnalysis;

internal static class ExternalFunctionSymbolExtensions
{
    public static string GetLibraryFilename(this ExternalFunctionSymbol symbol)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return string.Concat(symbol.Library, ".dll");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return string.Concat(symbol.Library, ".dylib");
        }
        else
        {
            return string.Concat(symbol.Library, ".so");
        }
    }
}
