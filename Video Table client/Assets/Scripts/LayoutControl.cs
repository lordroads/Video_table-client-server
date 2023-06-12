using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LayoutControl : MonoBehaviour
{
    public GameObject[] layouts;
    public VideoPlayer player;
    public Texture[] textureVideoButtons;

    private int _currentLayout;

    void Start()
    {
        if (layouts != null)
        {
            foreach (var layout in layouts)
            {
                if (layout == null || layout.name.Contains("Admin"))
                {
                    continue;
                }

                layout.SetActive(false);
            }

            layouts[0].SetActive(true);
            _currentLayout = 0;
        }
    }

    public void SetLayout(int number)
    {
        layouts[_currentLayout].SetActive(false);
        _currentLayout = number;
        layouts[_currentLayout].SetActive(true);
    }

    public void SetCurrentVideoButton(int number)
    {
        var layout = layouts[1];
        var rawImage = layout.GetComponentInChildren<RawImage>();
        if (rawImage != null)
        {
            rawImage.texture = textureVideoButtons[number];
        }
    }

    private void OnEnable()
    {
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
        Client.ActionReceivedMessage += DefaultLayout;
    }

    private void OnDisable()
    {
        SwipeDetector.OnSwipe -= SwipeDetector_OnSwipe;
        Client.ActionReceivedMessage += DefaultLayout;
    }

    private void DefaultLayout(string message)
    {
        if (_currentLayout == 1)
        {
            if (!string.IsNullOrEmpty(message) && message.Contains("False"))
            {
                UnityMainThread.wkr.AddJob(() =>
                {
                    SetLayout(0);
                });
            }
        }
    }

    public void CancelAdminPanel()
    {
        layouts[2].GetComponentInChildren<Animator>().SetBool("isOpen", false);
    }

    private void SwipeDetector_OnSwipe(SwipeData obj)
    {
        if (obj.Direction == SwipeDirection.Left)
        {
            layouts[2].GetComponentInChildren<Animator>().SetBool("isOpen", true);
        }
        else if (obj.Direction == SwipeDirection.Right)
        {
            layouts[2].GetComponentInChildren<Animator>().SetBool("isOpen", false);
        }
    }
}
