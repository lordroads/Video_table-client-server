using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SettingModel
{
    public string PathToVideoDirectory;
    public string IpAddress;
    public int Port;

    public bool IsFadeIn;
    public float SpeedFade;
    public float StepFade;
}
