using UnityEngine;

namespace MountainGoats.Core
{
    public static class MGLogger
    {
        public static void Log(object message, object context = null)
        {
            var contextName = context?.GetType().Name ?? "Unknown";
            Debug.Log($"[{contextName}] {message}");
        }

        public static void LogWarning(object message, object context = null)
        {
            var contextName = context?.GetType().Name ?? "Unknown";
            Debug.LogWarning($"[{contextName}] {message}");
        }

        public static void LogError(object message, object context = null)
        {
            var contextName = context?.GetType().Name ?? "Unknown";
            Debug.LogError($"[{contextName}] {message}");
        }
    }
} 