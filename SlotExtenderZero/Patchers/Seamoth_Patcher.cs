using Harmony;
using BZCommon;

namespace SlotExtenderZero.Patchers
{    
    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("slotIDs", MethodType.Getter)]
    internal class Seamoth_slotIDs_Patch
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref string[] __result)
        {            
            __result = SlotHelper.SessionSeamothSlotIDs;
            return false;
        }
    }    

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("Awake")]
    internal class SeaMoth_Awake_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(SeaMoth __instance)
        {
            __instance.gameObject.AddIfNeedComponent<SlotExtenderZero>();            
        }
    }
}
