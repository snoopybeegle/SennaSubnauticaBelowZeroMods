﻿using Harmony;
using BZCommon;
using UnityEngine;

namespace CheatManagerZer0
{
    internal class Patches
    {
        [HarmonyPatch(typeof(SeaMoth))]
        [HarmonyPatch("Awake")]
        internal class SeaMoth_Awake_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(SeaMoth __instance)
            {
                __instance.gameObject.AddIfNeedComponent<SeamothOverDrive>();               
            }
        }

        [HarmonyPatch(typeof(Exosuit))]
        [HarmonyPatch("Awake")]
        internal class Exosuit_Awake_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(Exosuit __instance)
            {
                __instance.gameObject.AddIfNeedComponent<ExosuitOverDrive>();
            }
        }

        [HarmonyPatch(typeof(Hoverbike))]
        [HarmonyPatch("Awake")]
        internal class Hoverbike_Start_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(Hoverbike __instance)
            {
                __instance.gameObject.AddIfNeedComponent<HoverbikeOverDrive>();
            }
        }

        /*
        [HarmonyPatch(typeof(CyclopsMotorMode))]
        [HarmonyPatch("Start")]
        internal class CyclopsMotorMode_Start_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(CyclopsMotorMode __instance)
            {
                __instance.gameObject.AddIfNeedComponent<CyclopsOverDrive>();
            }
        }
        */

        [HarmonyPatch(typeof(PlayerController))]
        [HarmonyPatch("SetMotorMode")]
        internal class PlayerController_SetMotorMode_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(PlayerController __instance, Player.MotorMode newMotorMode)
            {
                if (Main.Instance != null)
                {
                    Main.Instance.onPlayerMotorModeChanged.Trigger(newMotorMode);                    
                }
            }
        }

        [HarmonyPatch(typeof(DevConsole))]
        [HarmonyPatch("Submit")]
        internal class DevConsole_Submit_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(DevConsole __instance, string value, bool __result)
            {
                if (__result)
                {
                    if (Main.Instance != null)
                    {
                        lock (value)
                        {
                            Main.Instance.onConsoleCommandEntered.Trigger(value);
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(FiltrationMachine))]
        [HarmonyPatch("OnConsoleCommand_filterfast")]
        internal class FiltrationMachine_OnConsoleCommand_filterfast_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(FiltrationMachine __instance)
            {
                if (Main.Instance != null)
                {
                    Main.Instance.onFilterFastChanged.Trigger((bool)__instance.GetPrivateField("fastFiltering"));
                }
            }
        }

        [HarmonyPatch(typeof(Seaglide))]
        [HarmonyPatch("Start")]
        internal class Seaglide_Start_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(Seaglide __instance)
            {
                __instance.gameObject.AddIfNeedComponent<SeaglideOverDrive>();
            }
        }


        [HarmonyPatch(typeof(SeaTruckSegment))]
        [HarmonyPatch("Start")]
        internal class SeaTruckSegment_Start_Patch
        {
            [HarmonyPostfix]
            internal static void Postfix(SeaTruckSegment __instance)
            {
                __instance.gameObject.AddIfNeedComponent<SeaTruckOverDrive>();
            }
        }

    }
}
