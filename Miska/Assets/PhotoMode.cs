using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoMode : MonoBehaviour
{
    bool            m_photoModeActive;
    GameObject      m_photoOverlay;
    RectTransform   m_viewfinderTrans;
    Animator        m_animator;

    private void Start()
    {
        m_photoModeActive = false;
        m_photoOverlay = GameObject.Find("Photo_Overlay");
        //m_photoOverlay.SetActive(false);
        m_viewfinderTrans = m_photoOverlay.GetComponentInChildren<RectTransform>();
        m_animator = m_photoOverlay.GetComponentInChildren<Animator>();
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
                //m_photoOverlay.SetActive(true);
                m_animator.SetTrigger("TransIN");
            }
            else
            {
                m_animator.SetTrigger("TransOUT");
                Debug.Log("Photo Mode disengaged");
                m_photoModeActive = false;
            }
        }

        if(m_photoModeActive && Input.GetMouseButtonDown(0))
        {
            StartCoroutine("CapturePhoto");
        }

    }

    IEnumerator CapturePhoto()
    {
        m_photoOverlay.SetActive(false);
        yield return new WaitForSeconds(.05f);

        string fullpath = Application.dataPath + "/Photos/";
        DateTime now = DateTime.Now;
        string dt = now.Day.ToString() + "-" + now.Month.ToString() + "-" + now.Year.ToString() + " " + now.Hour.ToString() + "-" + now.Minute.ToString() + "-" + now.Second.ToString() + "-" + now.Millisecond.ToString();
        Debug.Log("Screen capture taken at " + dt);
        ScreenCapture.CaptureScreenshot(fullpath + "ScreenCap_test " + dt + ".png");
        yield return new WaitForSeconds(.05f);

        m_photoOverlay.SetActive(true);
    }

}
