namespace AudioLocker;

static class Constants
{
    public const int DEFAULT_AUDIO_SESSION_VOLUME_LEVEL = 10;
#if DEBUG
    public const string APP_NAME = "AudioLocker-Debug";
#else
    public const string APP_NAME = "AudioLocker";
#endif
}
