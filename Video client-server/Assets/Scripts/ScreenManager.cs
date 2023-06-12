using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace Assets.Scripts
{
    public class ScreenManager : MonoBehaviour
    {
        public GameObject TestLayout;
        public GameObject VideoLayout;

        public VideoControl VideoControl;

        public int lastCommand = 0;


        public void Awake()
        {
            ServerTcp.ActionReceivedMessage += PrintMessage;
            ServerTcp.ActionReceivedMessage += SetCommand;

            VideoControl.player.loopPointReached += StopVideo;
        }

        public void PrintMessage(string message)
        {
            Debug.Log($"Get for print method: {message}");
        }

        private void SetCommand(string command)
        {
            if (int.TryParse(command, out int result))
            {
                switch (result)
                {
                    case 0:
                        UnityMainThread.wkr.AddJob(() =>
                        {
                            TestLayout.SetActive(true);
                            VideoLayout.SetActive(false);
                            SaveLastCommand(result);
                        });
                        break;
                    case 1:
                        UnityMainThread.wkr.AddJob(() =>
                        {
                            TestLayout.SetActive(false);
                            VideoLayout.SetActive(true);
                            SaveLastCommand(result);
                        });
                        break;
                    default:
                        UnityMainThread.wkr.AddJob(() =>
                        {
                            VideoControl.Play(result - 2);
                        });
                        SaveLastCommand(result);
                    break;
                }
            }
        }

        private void StopVideo(VideoPlayer player)
        {
            ServerTcp.GetInstance().Send($"{lastCommand}:{false}");
        }

        private void SaveLastCommand(int command)
        {
            lastCommand = command;
        }
    }
}
