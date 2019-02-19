using Harmony;
using System;
using UnityEngine;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using BZCommon;
using QuickSlotExtenderZero.Configuration;
using UWE;

namespace QuickSlotExtenderZero
{
    public static class Main
    {        
        public const string PROGRAM_NAME = "QuickSlotExtenderZero";
        private static QSEConfig qsEConfig;
        public static bool isExists_SlotExdenerZero = false;
        public static QSHandler Instance { get; internal set; }

        public static void Load()
        {
            isExists_SlotExdenerZero = RefHelp.IsNamespaceExists("SlotExtenderZero");

            if (isExists_SlotExdenerZero)
                Debug.Log($"[{PROGRAM_NAME}] SlotExtenderZero found! trying to work together..");            

            try
            {        
                HarmonyInstance.Create("SubnauticaBelowZero.QuickSlotExtenderZero.mod").PatchAll(Assembly.GetExecutingAssembly());

                Debug.Log($"[{Main.PROGRAM_NAME}] Harmony Patches installed");
                
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
                //loading config from file
                Config.LoadConfig();
                Config.InitConfig();
                //add console commad for configuration window
                qsEConfig = new QSEConfig();
                //add an action if changed controls
                GameInput.OnBindingsChanged += GameInput_OnBindingsChanged;                
            }
        }

        internal static void GameInput_OnBindingsChanged()
        {
            //input changed, refreshing key bindings
            Config.InitSLOTKEYS();
        }

        public static object GetOtherModPublicField(string className, string fieldName, BindingFlags bindingFlags = BindingFlags.Default)
        {
            try
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                for (int i = 0; i < assemblies.Length; i++)
                {
                    Type[] types = assemblies[i].GetTypes();

                    for (int j = 0; j < types.Length; j++)
                    {
                        if (types[j].FullName == className)
                        {
                            return types[j].GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | bindingFlags).GetValue(types[j]);
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }    
}
