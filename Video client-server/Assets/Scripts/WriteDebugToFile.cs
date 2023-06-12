using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class WriteDebugToFile : MonoBehaviour
    {
        string filename = "";

        private void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        private void Log(string condition, string stackTrace, LogType type)
        {
            TextWriter tw = new StreamWriter(filename, true);

            tw.WriteLine($"[{DateTime.Now}]" + condition);

            tw.Close();
        }

        private void Awake()
        {
            filename =  Environment.CurrentDirectory + $"//Logs//log_{DateTime.Now.ToString("dd_MM_yy")}.txt";
        }


    }
}
