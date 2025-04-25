using UnityEngine;

namespace Core
{
    public static class MgLogger
    {
        public static bool IsDebugMode => Debug.isDebugBuild;
        
        public static void Log(object message, object context = null)
        {
            LogInternal(message, LogSeverity.Info, context);
        }

        public static void LogWarning(object message, object context = null)
        {
            LogInternal(message, LogSeverity.Warning, context);
        }

        public static void LogError(object message, object context = null)
        {
            LogInternal(message, LogSeverity.Error, context);
        }
        
        private static void LogInternal(object message, LogSeverity color, object context = null)
        {
            if (!IsDebugMode) return;
            
            var colorString = color switch
            {
                LogSeverity.Info => "#00FF00", // Green
                LogSeverity.Warning => "#FFFF00", // Yellow
                LogSeverity.Error => "#FF0000", // Red
                _ => "#FFFFFF", // White
            };
            
            var contextName = context?.GetType().Name ?? "Unknown";
            Debug.Log($"<color={colorString}>[{contextName}]</color> {message}");
        }

        private enum LogSeverity
        {
            Info,
            Warning,
            Error,
        }
    }
} 