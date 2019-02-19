using Harmony;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using BZCommon;
using SlotExtenderZero.Configuration;
using System.Reflection;

namespace SlotExtenderZero
{
    public static class Main
    {
        public const string PROGRAM_NAME = "SlotExtenderZero";
        public static HarmonyInstance hInstance;
        public static SEZConfig sEZConfig;

        internal static InputFieldListener ListenerInstance { get; set; }
        
        public static bool isConsoleActive;

        public static void Load()
        {
            try
            {
                //loading config from file
                Config.LoadConfig();
                SlotHelper.InitSlotIDs();

                hInstance = HarmonyInstance.Create("SubnauticaBelowZero.SlotExtenderZero.mod");
                
                hInstance.PatchAll(Assembly.GetExecutingAssembly());

                SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }                
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "StartScreen")
            {
                //enabling game console
                UnityHelper.EnableConsole();
                
                Config.InitConfig();
                //add console commad for configuration window
                sEZConfig = new SEZConfig();
                //add an action if changed controls
                GameInput.OnBindingsChanged += GameInput_OnBindingsChanged;                
            }
            if (scene.name == "Main")
            {
                //creating a console input field listener to skip SlotExdenderZero Update method key events conflict
                ListenerInstance = InitializeListener();
            }
        }

        internal static void GameInput_OnBindingsChanged()
        {
            //input changed, refreshing key bindings
            Config.InitSLOTKEYS();            
            
            if (Initialize_uGUI.Instance != null)
            {
                Initialize_uGUI.Instance.RefreshText();
            }            
        }        

        internal static InputFieldListener InitializeListener()
        {
            if (ListenerInstance == null)
            {
                ListenerInstance = UnityEngine.Object.FindObjectOfType(typeof(InputFieldListener)) as InputFieldListener;

                if (ListenerInstance == null)
                {
                    GameObject inputFieldListener = new GameObject().AddComponent<InputFieldListener>().gameObject;
                    inputFieldListener.name = "InputFieldListener";
                    ListenerInstance = inputFieldListener.GetComponent<InputFieldListener>();
                }
            }
            return ListenerInstance;
        }
    }    
}
