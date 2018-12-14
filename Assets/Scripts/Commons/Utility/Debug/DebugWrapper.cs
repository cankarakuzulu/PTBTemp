namespace nopact.Commons.Utility.Debug
{
    public static class DebugWrapper
    {

        public static void Assert(bool condition, string message)
        {
            if (UnityEngine.Debug.isDebugBuild)
            {
                if (!condition)
                {
                    UnityEngine.Debug.LogWarning(message);
                    UnityEngine.Debug.Break();
                }
            }
        }

        public static void Break()
        {
            if (UnityEngine.Debug.isDebugBuild)
            {
                UnityEngine.Debug.Break();
            }
        }

        public static void Log(object message)
        {
            if (UnityEngine.Debug.isDebugBuild)
            {
                UnityEngine.Debug.Log(message);
            }
        }

        public static void Log(object message, UnityEngine.Object context)
        {
            if (UnityEngine.Debug.isDebugBuild)
            {
                UnityEngine.Debug.Log(message, context);
            }
        }

        public static void LogError(object message)
        {
            if (UnityEngine.Debug.isDebugBuild)
            {
                UnityEngine.Debug.LogError(message);
            }
        }

        public static void LogError(object message, UnityEngine.Object context)
        {
            if (UnityEngine.Debug.isDebugBuild)
            {
                UnityEngine.Debug.LogError(message, context);
            }
        }

        public static void LogWarning(object message)
        {
            if (UnityEngine.Debug.isDebugBuild)
            {
                UnityEngine.Debug.LogWarning(message);
            }
        }

        public static void LogWarning(object message, UnityEngine.Object context)
        {
            if (UnityEngine.Debug.isDebugBuild)
            {
                UnityEngine.Debug.LogWarning(message, context);
            }
        }
    }
}
