namespace AudioLocker.Core.Loggers.Abstract;

public interface ILogger
{
    void Debug(string message);
    void Info(string message);
    void Warning(string message);
    void Error(string message);
    void Fatal(string message);
    void Debug(string message, Exception exception);
    void Info(string message, Exception exception);
    void Warning(string message, Exception exception);
    void Error(string message, Exception exception);
    void Fatal(string message, Exception exception);
}
