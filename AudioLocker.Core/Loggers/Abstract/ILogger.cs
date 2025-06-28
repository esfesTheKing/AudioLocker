using System.Runtime.CompilerServices;

namespace AudioLocker.Core.Loggers.Abstract;

public interface ILogger
{
    void Debug(string message, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null);
    void Info(string message, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null);
    void Warning(string message, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null);
    void Error(string message, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null);
    void Fatal(string message, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null);
    void Debug(string message, Exception exception, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null);
    void Info(string message, Exception exception, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null);
    void Warning(string message, Exception exception, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null);
    void Error(string message, Exception exception, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null);
    void Fatal(string message, Exception exception, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null);
}
