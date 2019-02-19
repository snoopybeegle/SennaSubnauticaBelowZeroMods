﻿using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using QuickSlotExtenderZero.Configuration;
using System.Collections.Generic;
using System;
using UWE;

namespace QuickSlotExtenderZero
{
    public class QSHandler : MonoBehaviour
    {
        public QSHandler Instance { get; private set; }
        public uGUI_QuickSlots quickSlots { get; private set; }
        public uGUI_ItemIcon[] icons { get; private set; }
        private bool islabelsAdded = false;
        public static object sezero_SLOTKEYSLIST;
        public List<string> SEZERO_SLOTKEYSLIST;        
        public Utils.MonitoredValue<bool> onConsoleInputFieldActive = new Utils.MonitoredValue<bool>();

        private bool isConsoleActive = false;

        public void Awake()
        {
            Instance = gameObject.GetComponent<QSHandler>();
            quickSlots = gameObject.GetComponent<uGUI_QuickSlots>();

            if (Main.isExists_SlotExdenerZero)
            {
                try
                {
                    sezero_SLOTKEYSLIST = Main.GetOtherModPublicField("SlotExtenderZero.Configuration.Config", "SLOTKEYSLIST", BindingFlags.Static);

                    SEZERO_SLOTKEYSLIST = new List<string>();

                    foreach (string item in (List<string>)sezero_SLOTKEYSLIST)
                    {
                        SEZERO_SLOTKEYSLIST.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        public void Start()
        {            
            onConsoleInputFieldActive.changedEvent.AddHandler(this, new Event<Utils.MonitoredValue<bool>>.HandleFunction(OnConsoleInputFieldActive));
        }

        private void OnConsoleInputFieldActive(Utils.MonitoredValue<bool> isActive)
        {
            isConsoleActive = isActive.value;            
        }

        public void OnDestroy()
        {
            onConsoleInputFieldActive.changedEvent.RemoveHandler(this, OnConsoleInputFieldActive);
            Destroy(Instance);
        }
        
        internal void Update()
        {
            if (isConsoleActive)
                return;

            if (Player.main == null)
                return;

            if (Inventory.main == null)
                return;           

            if (!islabelsAdded)
                AddQuickSlotText(quickSlots);

            if (Player.main.isPiloting)
               return;            

            if (Input.GetKeyDown(Config.KEYBINDINGS["Slot_6"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(5);
                return;
            }
            else if (Input.GetKeyDown(Config.KEYBINDINGS["Slot_7"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(6);
                return;
            }
            else if (Input.GetKeyDown(Config.KEYBINDINGS["Slot_8"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(7);
                return;
            }
            else if (Input.GetKeyDown(Config.KEYBINDINGS["Slot_9"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(8);
                return;
            }
            else if (Input.GetKeyDown(Config.KEYBINDINGS["Slot_10"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(9);
                return;
            }
            else if (Input.GetKeyDown(Config.KEYBINDINGS["Slot_11"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(10);
                return;
            }
            else if (Input.GetKeyDown(Config.KEYBINDINGS["Slot_12"]))
            {
                Inventory.main.quickSlots.SlotKeyDown(11);
                return;
            }
        }

        internal void AddQuickSlotText(uGUI_QuickSlots instance)
        {
            if (instance == null)
            {                
                return;
            }

            icons = (uGUI_ItemIcon[])instance.GetType().GetField("icons", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance);

            if (icons == null)
            {               
                return;
            }

            if (HandReticle.main == null)
            {                
                return;
            }

            for (int i = 0; i < icons.Length; i++)
            {                
                if (Main.isExists_SlotExdenerZero && Player.main.inSeamoth && Player.main.GetPDA().state != PDA.State.Opening)
                {
                    AddTextToSlot(icons[i].transform, SEZERO_SLOTKEYSLIST[i], i);                   
                }
                else if (Main.isExists_SlotExdenerZero && Player.main.inExosuit && Player.main.GetPDA().state != PDA.State.Opening)
                {
                    if (i < 2)
                    {
                        continue;
                    }
                    else
                    {
                        AddTextToSlot(icons[i].transform, SEZERO_SLOTKEYSLIST[i - 2], i);                        
                    }
                }
                else if (Player.main.inExosuit && Player.main.GetPDA().state != PDA.State.Opening)
                {
                    if (i < 2)
                    {
                        continue;
                    }
                    else
                    {
                        AddTextToSlot(icons[i].transform, Config.SLOTKEYSLIST[i - 2], i);
                    }
                }
                else
                {
                    AddTextToSlot(icons[i].transform, Config.SLOTKEYSLIST[i], i);
                }
            }

            islabelsAdded = true;
        }               
       
        //based on RandyKnapp's MoreQuickSlots Subnautica mod: "CreateNewText()" method
        //found on GitHub:https://github.com/RandyKnapp/SubnauticaModSystem

        private Text AddTextToSlot(Transform parent, string buttonText, int slotNum)
        {
            Text text = Instantiate(HandReticle.main.compTextHand);            
            text.gameObject.layer = parent.gameObject.layer;
            text.gameObject.name = $"Slot_{slotNum}";
            text.transform.SetParent(parent, false);
            text.transform.localScale = new Vector3(1, 1, 1);
            text.gameObject.SetActive(true);
            text.enabled = true;
            text.text = buttonText;
            text.fontSize = 17;
            text.color = Config.TEXTCOLOR;
            RectTransformExtensions.SetParams(text.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), parent);
            text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
            text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
            text.rectTransform.anchoredPosition = new Vector2(0, -40);
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;

            return text;
        }
    }
}
