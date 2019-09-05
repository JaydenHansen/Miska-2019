using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoMode : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot("ScreenCap_test");
        }

    }
}
