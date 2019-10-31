using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhotoMode : MonoBehaviour
{
    bool                            m_photoModeActive;
    GameObject                      m_photoOverlay;
    Animator                        m_animator;

    public GameObject               m_virtCamOBJ;
    Camera                          m_virtCam;
    PhotoSubject                    m_virtCamScript;
    public PhotoAlbum               m_photoAlbum;

    List<PhotoSubject>              m_capturedSubjects;
    List<PhotoSubject>              m_allSubjects;

    public GameObject               m_entry1,   m_entry2,   m_entry3; //Replace with array!!
    public PhotoSubject             m_subj1,    m_subj2,    m_subj3;

    public HUD_UI                   m_HUDUI;

    private void Start()
    {
        m_photoModeActive = false;
        m_photoOverlay = GameObject.Find("Photo_Overlay");
        //m_photoOverlay.SetActive(false);

        m_animator = m_photoOverlay.GetComponentInChildren<Animator>();

       // m_PopupOverlay = GameObject.Find("PopUpCanvas");

        m_virtCam = m_virtCamOBJ.GetComponent<Camera>();
        m_virtCamScript = m_virtCamOBJ.GetComponent<PhotoSubject>();
        m_virtCam.enabled = false;

        m_capturedSubjects = new List<PhotoSubject>();
        m_allSubjects = new List<PhotoSubject>();

        m_allSubjects.Add(m_subj1);
        m_allSubjects.Add(m_subj2);
        m_allSubjects.Add(m_subj3);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            togglePhotoModeActive();
        }


        if (m_photoModeActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine("CapturePhoto");
            }
        }

    }

    private void togglePhotoModeActive()
    {
        if (m_photoModeActive == false)
        {
            //m_PopupOverlay.SetActive(false);
            m_photoModeActive = true;
            //m_photoOverlay.SetActive(true);
            m_animator.SetTrigger("TransIN");
            //m_HUDUI.setActive(true); To Add
        }
        else
        {
           // m_PopupOverlay.SetActive(true);
            m_animator.SetTrigger("TransOUT");
            m_photoModeActive = false;
            //m_HUDUI.setActive(false); To Add
        }
    }



    IEnumerator CapturePhoto()
    {
        m_virtCam.enabled = true;
        m_photoOverlay.SetActive(false);
        yield return new WaitForSeconds(.05f);

        RenderToImage();
        //m_virtCamScript.RunIdentifier();
        yield return new WaitForSeconds(.05f);

        m_photoOverlay.SetActive(true);
        m_animator.Play("Active_Idle");
        m_virtCam.enabled = false;
    }

    private static string GenerateFileName()
    {

        string folderPath = Application.persistentDataPath;
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

        (bool isValidated, PhotoSubject subj) = isPhotoJournalValid();

        if (isValidated)
        {
            GameObject journalOBJ;
            bool poloroidFound = false;
            foreach (var lstdPhotos in m_capturedSubjects)
            {
                if (lstdPhotos == subj)
                {
                    journalOBJ = subj.getJournalEntry();
                    poloroidFound = true;
                }
            }

            if(!poloroidFound)
            {
                m_capturedSubjects.Add(subj);
                journalOBJ = GetJournalEntry();
            }
            else
            {
                journalOBJ = null;
            }

            subj.SetupPoloroid(filename, journalOBJ);
                //Run "journal alert"
        }
        else
        {
            m_photoAlbum.AddNewPhoto(filename);
        }
        //m_journalPhotos[m_currJournalPhoto].texture = LoadPNG(filename);
    }

    GameObject GetJournalEntry()
    {
        int count = m_capturedSubjects.Count;

        GameObject entryOBJ;

        if (count == 1)
        {
            entryOBJ = m_entry1;
        }
        else if (count == 2)
        {
            entryOBJ = m_entry2;
        }
        else if (count == 3)
        {
            entryOBJ = m_entry3;
        }
        else
        {
            entryOBJ = null;
        }

        return entryOBJ;
    }

    (bool, PhotoSubject) isPhotoJournalValid()
    {
        foreach (var subj in m_allSubjects)
        {
            if(subj.isPhotoValid())
            {
                return (true, subj);
            }
        }
        return (false, null);
    }

    Texture2D LoadPNG(string filepath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filepath))
        {
            fileData = File.ReadAllBytes(filepath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
}
