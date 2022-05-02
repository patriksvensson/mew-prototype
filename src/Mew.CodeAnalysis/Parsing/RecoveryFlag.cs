namespace Mew.CodeAnalysis;

[Flags]
internal enum RecoveryFlag
{
    None = 0,
    ConsumeTerminator = 1 << 0,
    SuppressDiagnostics = 1 << 1,
}
