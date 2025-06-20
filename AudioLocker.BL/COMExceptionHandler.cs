using AudioLocker.Core.Loggers.Abstract;
using System.Runtime.InteropServices;

namespace AudioLocker.BL;

internal class COMExceptionHandler(
        ILogger logger, 
        Action onKnownException, 
        Action<Exception> onUnknownException, 
        Action onCleanup
    )
{
    private const uint DEVICE_INVALIDATED_ERORR = 0x88890004;
    private const uint INVALID_HANDLE = 0x80070006;

    private readonly ILogger _logger = logger;
    
    private readonly Action _onKnownException = onKnownException;
    private readonly Action<Exception> _onUnknownException = onUnknownException;
    private readonly Action _onCleanup = onCleanup;

    public void HandleAccessExceptions(Action function)
    {
        try
        {
            function.Invoke();
        }
        catch (COMException exception)
        {
            var statusCode = unchecked((uint)exception.ErrorCode);
            if (statusCode != DEVICE_INVALIDATED_ERORR && statusCode != INVALID_HANDLE)
            {
                _onUnknownException.Invoke(exception);
                _logger.Warning("Unknown exception", exception);
            }

            _onCleanup.Invoke();
        }
    }
    public void HandleAccessExceptionsAsync(Func<Task> function)
    {
        var task = function.Invoke();
        try
        {
            task.Wait();
        }
        catch (AggregateException exception)
        {
            foreach (var innerException in exception.InnerExceptions)
            {
                if (innerException is COMException comException)
                {
                    var statusCode = unchecked((uint)comException.ErrorCode);
                    if (statusCode != DEVICE_INVALIDATED_ERORR && statusCode != INVALID_HANDLE)
                    {
                        _onUnknownException.Invoke(comException);
                        _logger.Warning("Unknown exception", exception);
                    }

                    continue;
                }
                else
                {
                    _logger.Warning("Unknown exception", exception);
                }

                _onCleanup.Invoke();
            }
        }
    }
}
