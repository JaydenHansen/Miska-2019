﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoMode : MonoBehaviour
{
    bool                                   m_photoModeActive;
    GameObject               m_photoOverlay;
    RectTransform        m_viewfinderTrans;
    Animator                      m_animator;
    GameObject              m_PopupOverlay;

    public GameObject          m_virtCamOBJ;
    Camera                                      m_virtCam;
    PhotoSubject                        m_virtCamScript;

    public PhotoAlbum m_album;

    private void Start()
    {
        m_photoModeActive = false;
        m_photoOverlay = GameObject.Find("Photo_Overlay");
        //m_photoOverlay.SetActive(false);
        m_viewfinderTrans = m_photoOverlay.GetComponentInChildren<RectTransform>();
        m_animator = m_photoOverlay.GetComponentInChildren<Animator>();
        m_PopupOverlay = GameObject.Find("PopUpCanvas");
        m_virtCam = m_virtCamOBJ.GetComponent<Camera>();
        m_virtCamScript = m_virtCamOBJ.GetComponent<PhotoSubject>();
        m_virtCam.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(m_photoModeActive == false)
            {
                m_PopupOverlay.SetActive(false);
                m_photoModeActive = true;
                Debug.Log("Photo Mode engaged");
                //m_photoOverlay.SetActive(true);
                m_animator.SetTrigger("TransIN");
            }
            else
            {
                m_PopupOverlay.SetActive(true);
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
        m_virtCam.enabled = true;
        m_photoOverlay.SetActive(false);
        yield return new WaitForSeconds(.05f);

        RenderToImage();
        m_virtCamScript.RunIdentifier();
        yield return new WaitForSeconds(.05f);

        m_photoOverlay.SetActive(true);
        m_animator.Play("Active_Idle");
        m_virtCam.enabled = false;
    }

    private static string GenerateFileName()
    {

        string folderPath = Application.dataPath + "/Resources/Photos/";
        DateTime now = DateTime.Now;
        string dt = now.Day.ToString() + "-" + now.Month.ToString() + "-" + now.Year.ToString() + " " + now.Hour.ToString() + "-" + now.Minute.ToString() + "-" + now.Second.ToString();
        //string ID

        string fullName = folderPath + dt + ".png"; //Add ID

        return fullName;
    }

    private void RenderToImage()
    {
        int sqrSide = 512;
        m_virtCam.aspect = 1.0f;

        RenderTexture tempRT = new RenderTexture(sqrSide, sqrSide, 24);

        m_virtCam.targetTexture = tempRT;
        m_virtCam.Render();

        RenderTexture.active = tempRT;
        Texture2D virtualPhoto = new Texture2D(sqrSide, sqrSide, TextureFormat.RGB24, false);
        virtualPhoto.ReadPixels(new Rect(0, 0, sqrSide, sqrSide), 0, 0);
               
        RenderTexture.active = null;
        m_virtCam.targetTexture = null;

        byte[] bytes;
        bytes = virtualPhoto.EncodeToPNG();

        string filename = GenerateFileName();

        System.IO.File.WriteAllBytes(filename, bytes);

        m_album.AddNewPhoto(filename);


     }
}
