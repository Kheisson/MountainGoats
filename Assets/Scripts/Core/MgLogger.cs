using UnityEngine;

namespace Core
{
    public static class MgLogger
    {
        public static void Log(object message, object context = null)
        {
            var contextName = context?.GetType().Name ?? "Unknown";
            Debug.Log($"<color=green>[{contextName}]</color> {message}");
        }

        public static void LogWarning(object message, object context = null)
        {
            var contextName = context?.GetType().Name ?? "Unknown";
            Debug.LogWarning($"<color=yellow>[{contextName}]</color> {message}");
        }

        public static void LogError(object message, object context = null)
        {
            var contextName = context?.GetType().Name ?? "Unknown";
            Debug.LogError($"<color=red>[{contextName}]</color> {message}");
        }
    }
} 