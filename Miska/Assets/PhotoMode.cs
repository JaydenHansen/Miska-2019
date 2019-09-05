using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoMode : MonoBehaviour
{
    bool            m_photoModeActive;
    GameObject      m_photoOverlay;
    RectTransform   m_viewfinderTrans;

    private void Start()
    {
        m_photoModeActive = false;
        m_photoOverlay = GameObject.Find("Photo_Overlay");
        m_photoOverlay.SetActive(false);
        m_viewfinderTrans = m_photoOverlay.GetComponentInChildren<RectTransform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(m_photoModeActive == false)
            {
                m_photoModeActive = true;
                Debug.Log("Photo Mode engaged");
                m_photoOverlay.SetActive(true);
                StartCoroutine("TransitionIN");
            }
            else
            {
                m_photoModeActive = false;
                Debug.Log("Photo Mode disengaged");
                StartCoroutine("TransitionOUT");
            }
        }

        if(m_photoModeActive && Input.GetMouseButtonDown(0))
        {
            m_photoOverlay.SetActive(false);
            DateTime now = DateTime.Now;
            string dt = now.Day.ToString() + "-" + now.Month.ToString() + "-" + now.Year.ToString() + " " + now.Hour.ToString() + "-" + now.Minute.ToString() + "-" + now.Second.ToString() + "-" + now.Millisecond.ToString();
            Debug.Log("Screen capture taken at " + dt);
            ScreenCapture.CaptureScreenshot("ScreenCap_test " + dt + ".png");
            m_photoOverlay.SetActive(true);
        }

    }

    IEnumerator TransitionIN()
    {
        for (float f = 20.0f; f <= 0.0f; f -=0.1f)
        {
            Quaternion quat = m_viewfinderTrans.rotation;
            quat.z = f;
            m_viewfinderTrans.rotation = quat;
            yield return null;
        }

    }

    IEnumerator TransitionOUT()
    {
        for (float f = 0.0f; f >= 20.0f; f += 0.1f)
        {
            Quaternion quat = m_viewfinderTrans.rotation;
            quat.z = f;
            m_viewfinderTrans.rotation = quat;
            yield return null;
        }
        m_photoOverlay.SetActive(false);
    }
}
