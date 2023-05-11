using System;
using UnityEngine;

namespace DearChar
{
    public class Debug
    {
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }
        public static void Log(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.Log(message, context);
        }

        public static void LogFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(format, args);
        }

        public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(context, format, args);
        }

        public static void LogFormat(LogType logType, LogOption logOptions, UnityEngine.Object context, string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(logType, logOptions, context, format, args);
        }
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public static void LogError(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError(message, context);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(format, args);
        }

        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        public static void LogWarning(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogWarning(message, context);
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(format, args);
        }

        public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(context, format, args);
        }

        public static void LogException(Exception exception)
        {
            UnityEngine.Debug.LogException(exception, null);
        }
        public static void LogException(Exception exception, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogException(exception, context);
        }
    }
}