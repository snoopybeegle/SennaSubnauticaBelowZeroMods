using System;
using System.Reflection;
using Harmony;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using BZCommon;
using CheatManagerZer0.Configuration;
using CheatManagerZer0.NewCommands;

namespace CheatManagerZer0
{
    public static class Main
    {
        public static CheatManagerZer0 Instance { get; private set; }
        public static CM_Logger CmLogger { get; private set; }
        public static CM_InfoBar CmInfoBar { get; private set; }

        internal static bool isConsoleEnabled { get; set; }
        internal static bool isInfoBarEnabled { get; set; }        
        
        //internal static bool isExistsSMLHelperV2;        

        public static void Load()
        {
            try
            {               
                HarmonyInstance.Create("SubnauticaBelowZer0.CheatManagerZer0.mod").PatchAll(Assembly.GetExecutingAssembly());
                UnityHelper.EnableConsole();
                SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);

                Config.InitConfig();                
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }

            //isExistsSMLHelperV2 = RefHelp.IsNamespaceExists("SMLHelper.V2");            
        }
        
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Main")
            {                
                Init();
                Instance.gameObject.AddIfNeedComponent<AlwaysDayConsoleCommand>();
                Instance.gameObject.AddIfNeedComponent<OverPowerConsoleCommand>();
            }
            else if (scene.name == "StartScreen")
            {
               // DisplayManager.OnDisplayChanged += OnDisplayChanged;               
                
                if (isInfoBarEnabled)
                    CmInfoBar = new CM_InfoBar();

                if (isConsoleEnabled)
                    CmLogger = new CM_Logger();

                CmConfig.Load();
            }
        }

        public static void OnDisplayChanged()
        {
            Debug.Log($"Resolution changed!");
        }

        public static CheatManagerZer0 Init()
        {
            if (Instance == null)
            {
                Instance = UnityEngine.Object.FindObjectOfType(typeof(CheatManagerZer0)) as CheatManagerZer0;

                if (Instance == null)
                {
                    GameObject cheatManagerZer0 = new GameObject().AddComponent<CheatManagerZer0>().gameObject;
                    cheatManagerZer0.name = "CheatManagerZer0";
                    Instance = cheatManagerZer0.GetComponent<CheatManagerZer0>();
                }
            }
            return Instance;
        }
    }  
}
