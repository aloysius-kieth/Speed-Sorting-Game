using System.Diagnostics;
public static class Logger
{
    [Conditional("ENABLE_LOGS")]
    public static void Debug(object logMsg)
    {
        UnityEngine.Debug.Log(logMsg);
    }

    [Conditional("ENABLE_LOGS")]
    public static void DebugWarning(object logMsg)
    {
        UnityEngine.Debug.LogWarning(logMsg);
    }

    [Conditional("ENABLE_LOGS")]
    public static void DebugError(object logMsg)
    {
        UnityEngine.Debug.LogError(logMsg);
    }
}