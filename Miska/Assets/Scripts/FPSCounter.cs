using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Debug fps counter
/// </summary>
public class FPSCounter : MonoBehaviour
{
    public int avgFrameRate;
    public Text display_Text;

    float m_timer;
    int m_frameCount;
    float m_totalFrameTime;
    void Start()
    {
        // disables vsyncs and removes the target fps
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
        Debug.Log("vSync = " + QualitySettings.vSyncCount.ToString());
        Debug.Log("Target Frame Rate = " + Application.targetFrameRate.ToString());
    }

    /// <summary>
    /// Takes the average frame time over a second
    /// </summary>
    public void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= 1)
        {
            m_timer = 0; // resets the timer
            
            float average = m_totalFrameTime / m_frameCount; // gets the average frame timer
            avgFrameRate = (int)(1f / average); // gets the average frame rate
            display_Text.text = avgFrameRate.ToString() + " FPS"; // displays the fps

            // reset
            m_totalFrameTime = 0;
            m_frameCount = 0;
        }
        else
        {
            m_totalFrameTime += Time.unscaledDeltaTime;
            m_frameCount++; // keeps count of the amount of frames to get the average
        }
    }
}
