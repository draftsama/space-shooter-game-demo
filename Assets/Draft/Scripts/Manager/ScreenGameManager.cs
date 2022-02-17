using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Draft.Manager
{
    public class ScreenGameManager : MonoBehaviour
    {
        private const float SCREEN_WIDTH = 1920f;
        private const float SCREEN_HEIGHT = 1080f;
        void Awake()
        {
            var res = Screen.currentResolution;
            var ratio = SCREEN_HEIGHT / SCREEN_WIDTH;
            Screen.SetResolution(Mathf.RoundToInt(res.height * ratio), res.height, false);
        }

    }
}