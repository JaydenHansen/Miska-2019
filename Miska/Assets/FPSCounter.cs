using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public int avgFrameRate;
    public Text display_Text;

    float m_timer;
    int m_frameCount;
    float m_totalFrameTime;
    void Start()
    {
        Debug.Log("vSync = " + QualitySettings.vSyncCount.ToString());
        QualitySettings.vSyncCount = 0;
        Debug.Log("Target Frame Rate = " + Application.targetFrameRate.ToString());
    }

    public void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= 1)
        {
            m_timer = 0;
            
            float average = m_totalFrameTime / m_frameCount;
            avgFrameRate = (int)(1f / average);
            display_Text.text = avgFrameRate.ToString() + " FPS";

            m_totalFrameTime = 0;
            m_frameCount = 0;
        }
        else
        {
            m_totalFrameTime += Time.unscaledDeltaTime;
            m_frameCount++;
        }
    }
}
