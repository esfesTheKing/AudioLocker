﻿using System.Runtime.InteropServices;

namespace AudioLocker.BL;

internal class COMExceptionHandler
{
    private const uint DEVICE_INVALIDATED_ERORR = 0x88890004;
    private const uint INVALID_HANDLE = 0x80070006;

    private readonly Action _onKnownException;
    private readonly Action<Exception> _onUnknownException;
    private readonly Action _onCleanup;

    public COMExceptionHandler(Action onKnownException, Action<Exception> onUnknownException, Action onCleanup)
    {
        _onKnownException = onKnownException;
        _onUnknownException = onUnknownException;
        _onCleanup = onCleanup;
    }

    public void HandleSessionAccessExceptions(Action function)
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
            }

            _onCleanup.Invoke();
        }
    }
    public void HandleSessionAccessExceptionsAsync(Func<Task> function)
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
                    }
                    continue;
                }

                _onCleanup.Invoke();
            }
        }
    }
}
