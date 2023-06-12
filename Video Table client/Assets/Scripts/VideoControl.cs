using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
    public class VideoControl : MonoBehaviour
    {
        public VideoPlayer player;
        public RenderTexture texture;

        public RawImage rawImage;
        public bool IsFadeIn = true;
        public float speedFade = 0.1f;
        public float stepFade = 0.1f;

        public VideoClip[] PathToFiles;

        public void Start()
        {
            player.clip = PathToFiles[0];

            IsFadeIn = SettingController.Settings.IsFadeIn;
            stepFade = SettingController.Settings.StepFade;
            speedFade = SettingController.Settings.SpeedFade;
        }
        public void Play(int number)
        {
            if (number <= PathToFiles.Length - 1)
            {
                player.Stop();

                texture.Release();

                player.clip = PathToFiles[number];

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
    }
}
