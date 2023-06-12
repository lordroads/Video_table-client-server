using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

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
        try
        {
            string path = Application.persistentDataPath + "/setting.json";

            Debug.Log($"Created & Save - {path}");
            File.WriteAllText(path, JsonUtility.ToJson(model));

        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        
    }

    [ContextMenu("Load")]
    public static SettingModel Load()
    {
        try
        {
            string path = Application.persistentDataPath + "/setting.json";

            if (File.Exists(path))
            {
                Debug.Log($"Load - {path}");
                return JsonUtility.FromJson<SettingModel>(File.ReadAllText(path));
            }
            return new SettingModel
            {
                IpAddress = "127.0.0.1",
                Port = 5000
            };
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return new SettingModel
            {
                IpAddress = "127.0.0.1",
                Port = 5000
            };
        }
    }

    private void OnApplicationQuit()
    {
        Save(Settings);
    }
}
