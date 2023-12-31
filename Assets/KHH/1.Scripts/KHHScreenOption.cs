﻿using UnityEngine;

public class KHHScreenOption : MonoBehaviour
{
#if !UNITY_EDITOR
    bool isFullScreen = false;

    // Update is called once per frame
    void Update()
    {
        //알트 + 엔터
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Return))
        {
            isFullScreen = !isFullScreen;
            if(isFullScreen)
            {
                Screen.SetResolution(1920, 1600, true);
            }
            else
            {
                Screen.SetResolution(1600, 900, false);
            }
        }
    }
#endif
}
