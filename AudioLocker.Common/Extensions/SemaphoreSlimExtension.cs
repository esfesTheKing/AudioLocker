namespace AudioLocker.Common.Extenstions
{
    public static class SemaphoreSlimExtension
    {
        public static async Task LockAsync(this SemaphoreSlim semaphoreSlim, Func<Task> task)
        {
            await semaphoreSlim.WaitAsync();

            try
            {
                await task();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
