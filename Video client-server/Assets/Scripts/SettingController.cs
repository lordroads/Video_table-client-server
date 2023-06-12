using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class SettingController : MonoBehaviour
    {
        public static SettingModel Settings = new SettingModel();

        public SettingModel settingForView; //для просмотра в редакторе Unity

        void Awake()
        {
            Settings = Load();
            settingForView = Settings; //для просмотра в редакторе Unity
        }

        [ContextMenu("Save")]
        public static void Save(SettingModel model)
        {
            Debug.Log($"Save - {Environment.CurrentDirectory + "/Service/setting.json"}");
            File.WriteAllText(Environment.CurrentDirectory + "/Service/setting.json", JsonUtility.ToJson(model));
        }

        [ContextMenu("Load")]
        public static SettingModel Load()
        {
            Debug.Log($"Load - {Environment.CurrentDirectory + "/Service/setting.json"}");
            return JsonUtility.FromJson<SettingModel>(File.ReadAllText(Environment.CurrentDirectory + "/Service/setting.json"));
        }
    }
}
