using UnityEngine;

namespace SlotExtenderZero.Configuration
{
    public class SEZConfig : MonoBehaviour
    {
        private const string Message = "Information: Enter 'sezconfig' command for configuration window.";

        public SEZConfig Instance { get; private set; }
        
        public void Awake()
        {
            Instance = this;           
            DevConsole.RegisterConsoleCommand(this, "sezconfig", false, false);
            Logger.Log(Message);
        }
        
        private void OnConsoleCommand_sezconfig(NotificationCenter.Notification n)
        {
            ConfigUI configUI = new ConfigUI();
        }

        public SEZConfig ()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType(typeof(SEZConfig)) as SEZConfig;

                if (Instance == null)
                {
                    GameObject sezconfig_command = new GameObject().AddComponent<SEZConfig>().gameObject;
                    sezconfig_command.name = "SEZConfig";
                    Instance = sezconfig_command.GetComponent<SEZConfig>();
                }
            }            
        }
    }
}
