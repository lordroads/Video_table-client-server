using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Assets.Scripts
{
    public class VideoControl : MonoBehaviour
    {
        public ServerTcp serverTcp;

        public VideoPlayer player;
        public RenderTexture texture;
        public RawImage rawImage;

        public string[] PathToFiles;

        public string PathToDirectory;

        public bool IsFadeIn;
        public float speedFade;
        public float stepFade;

        public void Start()
        {
            PathToDirectory = SettingController.Settings.PathToVideoDirectory;
            IsFadeIn = SettingController.Settings.IsFadeIn;
            speedFade = SettingController.Settings.SpeedFade;
            stepFade = SettingController.Settings.StepFade;


            PathToFiles = Directory.GetFiles(PathToDirectory);
            player.url = PathToFiles[0];

            player.loopPointReached += DefaultPlayback;
        }

        public void Play(int number)
        {
            if (number <= PathToFiles.Length - 1)
            {
                player.Stop();

                texture.Release();

                player.url = PathToFiles[number];

                player.Play();

                if (IsFadeIn)
                {
                    var color = rawImage.color;
                    color.a = 0;
                    rawImage.color = color;

                    StartCoroutine(FadeIn(color));
                }
            }
        }

        IEnumerator FadeIn(Color color)
        {
            while (rawImage.color.a <= 1)
            {
                color.a += stepFade;

                rawImage.color = color;

                Debug.Log($"Color a = {color.a}");

                yield return new WaitForSeconds(speedFade);
            }
        }

        public void DefaultPlayback(VideoPlayer player)
        {
            Play(0);
        }
    }
}
