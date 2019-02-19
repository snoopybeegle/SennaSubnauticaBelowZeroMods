﻿using Harmony;

namespace SlotExtenderZero.Patchers
{
    [HarmonyPatch(typeof(DevConsole))]
    [HarmonyPatch("SetState")]
    internal class DevConsole_SetState_Patch
    {
        [HarmonyPrefix]
        internal static void Prefix(DevConsole __instance, bool value)
        {
            if (Main.ListenerInstance != null)
            {
                Main.ListenerInstance.onConsoleInputFieldActive.Update(value);
            }
        }
    }
}
