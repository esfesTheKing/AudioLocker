using Serilog.Context;
using System.Runtime.CompilerServices;

using AudioLockerILogger = AudioLocker.Core.Loggers.Abstract.ILogger;

namespace AudioLocker.BL.Loggers;

public class SerilogLogger(Serilog.ILogger logger) : AudioLockerILogger
{
    private readonly Serilog.ILogger _logger = logger;

    public void Debug(string message, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null)
    {
        using var logContextClassProperty = LogContext.PushProperty("Class", Path.GetFileNameWithoutExtension(callerFilePath) ?? "Unknown Class");
        using var logContextMethodProperty = LogContext.PushProperty("Method", callingMethod ?? "Method Unknown");

        _logger.Debug(message);
    }

    public void Debug(string message, Exception exception, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null)
    {
        using var logContextClassProperty = LogContext.PushProperty("Class", Path.GetFileNameWithoutExtension(callerFilePath) ?? "Unknown Class");
        using var logContextMethodProperty = LogContext.PushProperty("Method", callingMethod ?? "Method Unknown");

        _logger.Debug(exception, message);
    }

    public void Error(string message, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null)
    {
        using var logContextClassProperty = LogContext.PushProperty("Class", Path.GetFileNameWithoutExtension(callerFilePath) ?? "Unknown Class");
        using var logContextMethodProperty = LogContext.PushProperty("Method", callingMethod ?? "Method Unknown");

        _logger.Error(message);
    }

    public void Error(string message, Exception exception, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null)
    {
        using var logContextClassProperty = LogContext.PushProperty("Class", Path.GetFileNameWithoutExtension(callerFilePath) ?? "Unknown Class");
        using var logContextMethodProperty = LogContext.PushProperty("Method", callingMethod ?? "Method Unknown");

        _logger.Error(exception, message);
    }

    public void Fatal(string message, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null)
    {
        using var logContextClassProperty = LogContext.PushProperty("Class", Path.GetFileNameWithoutExtension(callerFilePath) ?? "Unknown Class");
        using var logContextMethodProperty = LogContext.PushProperty("Method", callingMethod ?? "Method Unknown");

        _logger.Fatal(message);
    }

    public void Fatal(string message, Exception exception, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null)
    {
        using var logContextClassProperty = LogContext.PushProperty("Class", Path.GetFileNameWithoutExtension(callerFilePath) ?? "Unknown Class");
        using var logContextMethodProperty = LogContext.PushProperty("Method", callingMethod ?? "Method Unknown");

        _logger.Fatal(exception, message);
    }

    public void Info(string message, [CallerFilePath] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null)
    {
        using var logContextClassProperty = LogContext.PushProperty("Class", Path.GetFileNameWithoutExtension(callerFilePath) ?? "Unknown Class");
        using var logContextMethodProperty = LogContext.PushProperty("Method", callingMethod ?? "Method Unknown");

        _logger.Information(message);
    }

    public void Info(string message, Exception exception, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null)
    {
        using var logContextClassProperty = LogContext.PushProperty("Class", Path.GetFileNameWithoutExtension(callerFilePath) ?? "Unknown Class");
        using var logContextMethodProperty = LogContext.PushProperty("Method", callingMethod ?? "Method Unknown");

        _logger.Information(exception, message);
    }

    public void Warning(string message, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null)
    {
        using var logContextClassProperty = LogContext.PushProperty("Class", Path.GetFileNameWithoutExtension(callerFilePath) ?? "Unknown Class");
        using var logContextMethodProperty = LogContext.PushProperty("Method", callingMethod ?? "Method Unknown");

        _logger.Warning(message);
    }

    public void Warning(string message, Exception exception, [CallerMemberName] string? callingMethod = null, [CallerFilePath] string? callerFilePath = null)
    {
        using var logContextClassProperty = LogContext.PushProperty("Class", Path.GetFileNameWithoutExtension(callerFilePath) ?? "Unknown Class");
        using var logContextMethodProperty = LogContext.PushProperty("Method", callingMethod ?? "Method Unknown");

        _logger.Warning(exception, message);
    }
}
