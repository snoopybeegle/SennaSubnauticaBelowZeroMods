﻿using System;
using System.Collections.Generic;
using SlotExtenderZero.Configuration;

namespace SlotExtenderZero
{
    internal static class SlotHelper
    {
        private static bool SlotMappingExpanded = false;

        internal static string[] SessionSeamothSlotIDs { get; private set; }
        internal static string[] SessionExosuitSlotIDs { get; private set; }

        internal static readonly string[] ExpandedSeamothSlotIDs = new string[12]
        {
            "SeamothModule1",
            "SeamothModule2",
            "SeamothModule3",
            "SeamothModule4",
            // New slots start here
            "SeamothModule5",
            "SeamothModule6",
            "SeamothModule7",
            "SeamothModule8",
            "SeamothModule9",
            "SeamothModule10",
            "SeamothModule11",
            "SeamothModule12"
        };

        internal static readonly string[] ExpandedExosuitSlotIDs = new string[14]
        {
            "ExosuitArmLeft",
            "ExosuitArmRight",
            "ExosuitModule1",
            "ExosuitModule2",
            "ExosuitModule3",
            "ExosuitModule4",
            // New slots start here
            "ExosuitModule5",
            "ExosuitModule6",
            "ExosuitModule7",
            "ExosuitModule8",
            "ExosuitModule9",
            "ExosuitModule10",
            "ExosuitModule11",
            "ExosuitModule12"
        };        

        internal static IEnumerable<string> NewSeamothSlotIDs
        {
            get
            {
                for (int i = 4; i < Config.MAXSLOTS; i++)
                {
                    yield return ExpandedSeamothSlotIDs[i];
                }
            }
        }        

        internal static IEnumerable<string> NewExosuitSlotIDs
        {
            get
            {
                for (int i = 6; i < Config.MAXSLOTS + 2; i++)
                {
                    yield return ExpandedExosuitSlotIDs[i];
                }
            }
        }
        
        internal static void InitSlotIDs()
        {
            SessionSeamothSlotIDs = (string[])Array.CreateInstance(typeof(string), Config.MAXSLOTS);
            SessionExosuitSlotIDs = (string[])Array.CreateInstance(typeof(string), Config.MAXSLOTS + 2);

            for (int i = 0; i < Config.MAXSLOTS; i++)
            {
                SessionSeamothSlotIDs[i] = ExpandedSeamothSlotIDs[i];               
            }

            for (int i = 0; i < Config.MAXSLOTS + 2; i++)
            {                
                SessionExosuitSlotIDs[i] = ExpandedExosuitSlotIDs[i];
            }
        }

        internal static void ExpandSlotMapping()
        {
            if (!SlotMappingExpanded)
            {
                foreach (string slotID in NewSeamothSlotIDs)
                    Equipment.slotMapping.Add(slotID, EquipmentType.SeamothModule);

                foreach (string slotID in NewExosuitSlotIDs)
                    Equipment.slotMapping.Add(slotID, EquipmentType.ExosuitModule);

                Logger.Log("Equipment slot mapping Patched!");
                SlotMappingExpanded = true;
            }
        }
    }
}
