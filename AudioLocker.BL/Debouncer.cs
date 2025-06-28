namespace AudioLocker.BL;

public static class Debouncer
{
    private const int DEFAULT_DEBOUNCE_TIME_MS = 10;

    private readonly static Dictionary<string, CancellationTokenSource?> _tokens = [];

    public static void Debounce(string id, Action action, int debounce_ms = DEFAULT_DEBOUNCE_TIME_MS)
    {
        _tokens.TryGetValue(id, out var token);

        token?.Cancel();
        token = new CancellationTokenSource();

        _tokens[id] = token;

        Task.Delay(debounce_ms, token.Token).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                try
                {
                    action.Invoke();
                }
                finally
                {
                    _tokens.Remove(id);
                }
            }
        });
    }
}
