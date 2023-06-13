using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataView : MonoBehaviour
{
    public InputField ipAddres;
    public InputField port;
    public Toggle isFade;
    public Slider stepFade;
    public Slider speedFade;


    private SettingModel settings;


    void Start()
    {
        settings = SettingController.Settings;

        ipAddres.text = settings.IpAddress;
        port.text = settings.Port.ToString();
        isFade.isOn = settings.IsFadeIn;
        stepFade.value = settings.StepFade;
        speedFade.value = settings.SpeedFade;
    }

    public void SaveChangeValue()
    {
        Client.GetInstance().Send("off");

        if(!string.IsNullOrEmpty(ipAddres.text) && ValidateIPv4(ipAddres.text))
        {
            settings.IpAddress = ipAddres.text;
        }

        if (int.TryParse(port.text, out int result))
        {
            settings.Port = result;
        }

        settings.IsFadeIn = isFade.isOn;
        settings.StepFade = stepFade.value;
        settings.SpeedFade = speedFade.value;

        SettingController.Save(settings);

        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    private bool ValidateIPv4(string ipString)
    {
        if (String.IsNullOrWhiteSpace(ipString))
        {
            return false;
        }

        string[] splitValues = ipString.Split('.');
        if (splitValues.Length != 4)
        {
            return false;
        }

        byte tempForParsing;

        return splitValues.All(r => byte.TryParse(r, out tempForParsing));
    }

}
