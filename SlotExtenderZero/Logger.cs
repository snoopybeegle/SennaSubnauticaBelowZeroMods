using UnityEngine;

namespace SlotExtenderZero
{
    internal static class Logger
    {
        internal static void Log(string message)
        {
            Debug.Log($"[SlotExtenderZero] {message}");
        }

        internal static void Log(string format, params object[] args)
        {
            Debug.Log("[SlotExtenderZero] " + string.Format(format, args));
        }
    }
}
