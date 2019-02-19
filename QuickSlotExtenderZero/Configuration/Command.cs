using UnityEngine;

namespace QuickSlotExtenderZero.Configuration
{
    public class QSEConfig : MonoBehaviour
    {
        private const string Message = "Information: Enter 'qseconfig' command for configuration window.";

        public QSEConfig Instance { get; private set; }
        
        public void Awake()
        {
            Instance = this;           
            DevConsole.RegisterConsoleCommand(this, "qseconfig", false, false);
            Debug.Log($"[{Main.PROGRAM_NAME}] {Message}");
        }
        
        private void OnConsoleCommand_qseconfig(NotificationCenter.Notification n)
        {
            ConfigUI configUI = new ConfigUI();
        }

        public QSEConfig ()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType(typeof(QSEConfig)) as QSEConfig;

                if (Instance == null)
                {
                    GameObject qseconfig_command = new GameObject().AddComponent<QSEConfig>().gameObject;
                    qseconfig_command.name = "QSEConfig";
                    Instance = qseconfig_command.GetComponent<QSEConfig>();
                }
            }            
        }
    }
}
