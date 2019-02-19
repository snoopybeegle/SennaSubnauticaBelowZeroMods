using Harmony;
using System.Reflection;
using System;
using UnityEngine;
using QuickSlotExtenderZero.Configuration;
using BZCommon;

namespace QuickSlotExtenderZero.Patchers
{
    [HarmonyPatch(typeof(QuickSlots))]
    [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(GameObject), typeof(Transform), typeof(Transform), typeof(Inventory), typeof(Transform), typeof(int) })]
    internal class QuickSlots_Constructor_Patch
    {
        static bool isPatched = false;

        [HarmonyPrefix]
        internal static void Prefix(QuickSlots __instance, GameObject owner, Transform toolSocket, Transform cameraSocket, Inventory inv, Transform slotTr, ref int slotCount)
        {
            if (isPatched)
                return;

            __instance.GetType().GetField("slotNames", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.SetField).SetValue(__instance, QuickSlotHelper.ExpandedQuickSlotNames);

            slotCount = Config.MAXSLOTS;

            isPatched = true;
        }
    }

    [HarmonyPatch(typeof(uGUI_QuickSlots))]
    [HarmonyPatch("Init")]
    internal class uGUI_QuickSlots_Init_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(uGUI_QuickSlots __instance)
        {
            Main.Instance = __instance.gameObject.AddOrGetComponent<QSHandler>();
            Main.Instance.AddQuickSlotText(__instance);
        }
    }

    [HarmonyPatch(typeof(DevConsole))]
    [HarmonyPatch("SetState")]
    internal class DevConsole_SetState_Patch
    {
        [HarmonyPrefix]
        internal static void Prefix(DevConsole __instance, bool value)
        {          
            if (Main.Instance != null)
            {                
                Main.Instance.onConsoleInputFieldActive.Update(value);                
            }           
        }
    }
}
