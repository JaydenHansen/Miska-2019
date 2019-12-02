using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Photo Capture Mode, including animation, image capture & sending
/// </summary>
public class PhotoMode : MonoBehaviour
{
    private         bool                m_photoModeActive;
    private         GameObject          m_photoOverlay;
    private         Animator            m_animator;

    public          GameObject          m_virtCamOBJ;
    private         Camera              m_virtCam;
    private         PhotoSubject        m_virtCamScript;
    public          PhotoAlbum          m_photoAlbum;

    private         List<PhotoSubject>  m_capturedSubjects;
    private         List<PhotoSubject>  m_allSubjects;

    public          GameObject          m_entry1,   m_entry2,   m_entry3; 
    public          PhotoSubject        m_subj1,    m_subj2,    m_subj3;

    public          HUD_UI              m_HUDUI;
    public          AK.Wwise.Event      m_captureSound;
    public          AKStateToggle       m_AmbienceToggle;

    private void Awake()
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

    /// <summary>
    /// Toggles Photo Mode on and off
    /// </summary>
    private void togglePhotoModeActive()
    {
        if (m_photoModeActive == false)
        {
            m_photoModeActive = true;
            m_animator.SetTrigger("TransIN");
        }
        else
        {
            m_animator.SetTrigger("TransOUT");
            m_photoModeActive = false;
        }
    }


    /// <summary>
    /// Manages photo capture (toggling photo viewfinder overlay, virtual camera and intiating photo capture)
    /// </summary>
    IEnumerator CapturePhoto()
    {
        m_virtCam.enabled = true;                   //Enables virtual camera to capture image (no UI)   --> both are reverted after image capture
        m_photoOverlay.SetActive(false);            //Disables photo overlay for visual feedback
        yield return new WaitForSeconds(.05f);

        RenderToImage();
        m_captureSound.Post(gameObject);
        yield return new WaitForSeconds(.05f);

        m_photoOverlay.SetActive(true);
        m_animator.Play("Active_Idle");
        m_virtCam.enabled = false;
    }

    /// <summary>
    /// Creates a file name string
    /// </summary>
    private static string GenerateFileName(bool valid, PhotoSubject subject)
    {
        string prefix = "";

        if (valid)
        {
            var picOBJ = subject.getSubject();
            if (picOBJ == JournalSubject.LOC_River)
            {
                prefix = "RIV";
            }
            else if (picOBJ == JournalSubject.LOC_Fireflies)
            {
                prefix = "FLY";
            }
            else if (picOBJ == JournalSubject.LOC_DuckPond)
            {
                prefix = "DCK";
            }
        }

        string folderPath = Application.persistentDataPath + "/Photos";
        DateTime now = DateTime.Now;
        string dt = now.Day.ToString() + "-" + now.Month.ToString() + "-" + now.Year.ToString() + " " + now.Hour.ToString() + "-" + now.Minute.ToString() + "-" + now.Second.ToString();

        string fullName = folderPath + "/" + prefix + dt + ".png";

        return fullName;
    }


    /// <summary>
    /// Renders the photograph to a .png file
    /// </summary>
    private void RenderToImage()
    {
        int sqrSide = 512;
        m_virtCam.aspect = 1.0f;

        RenderTexture tempRT = new RenderTexture(sqrSide, sqrSide, 24); 

        
        m_virtCam.targetTexture = tempRT;                                                       //Renders the image to a temporary texutre
        m_virtCam.Render();

        RenderTexture.active = tempRT;
        Texture2D virtualPhoto = new Texture2D(sqrSide, sqrSide, TextureFormat.RGB24, false);   //Conversion to Texture 2D
        virtualPhoto.ReadPixels(new Rect(0, 0, sqrSide, sqrSide), 0, 0);
               
        RenderTexture.active = null;
        m_virtCam.targetTexture = null;

        byte[] bytes;
        bytes = virtualPhoto.EncodeToPNG();                                                     //Encoded to png File

        (bool isValidated, PhotoSubject subj) = isPhotoJournalValid();

        string filename = GenerateFileName(isValidated, subj);

        System.IO.File.WriteAllBytes(filename, bytes);                                          //Saved to system




        if (isValidated)
        {
            AttachToJournal(subj, filename);
            subj.SetActiveState(false);
        }
        else
        {
            m_photoAlbum.AddNewPhoto(filename);
        }
    }

    public void AttachToJournal(PhotoSubject subj, string filename)
    {
        GameObject journalOBJ;
        bool poloroidFound = false;
        foreach (var lstdPhotos in m_capturedSubjects)  //For loop checks that there is no photo has been taken of the subject
        {
            if (lstdPhotos == subj)
            {
                journalOBJ = subj.getJournalEntry();
                poloroidFound = true;
            }
        }

        if (!poloroidFound)                              //if no photo has been taken previous, add subject to captured list, and get next journal entry
        {
            m_capturedSubjects.Add(subj);
            journalOBJ = GetNextAvailableJournalEntry();
        }
        else                                            //Otherwise don't provide a journal object 
        {
            journalOBJ = null;
        }

        subj.SetupPoloroid(filename, journalOBJ);
        //Run "journal alert"
    }

    /// <summary>
    /// Loading in photos from file and appending to journal entry
    /// </summary>
    /// <param name="js">subject of the journal photo</param>
    /// <param name="fi">file info of the import photo</param>
    public void LoadInJournalEntry(JournalSubject js, FileInfo fi)
    {
        int num = m_capturedSubjects.Count;
        PhotoSubject ps = m_allSubjects[num];
        m_capturedSubjects.Add(ps);
        GameObject journalOBJ = GetNextAvailableJournalEntry();
        ps.SetupPoloroid(fi.Name, journalOBJ);

    }

    /// <summary>
    /// Finds next available Journal Entry object (which contains text and photo image objects
    /// </summary>
    GameObject GetNextAvailableJournalEntry()
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

    /// <summary>
    /// checks if photo is "journal valid" and the subject
    /// </summary>
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

    /// <summary>
    /// Returns a texture from a png at a given file path
    /// </summary>
    /// <param name="filepath"> file path of photo being loaded</param>
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

    public void ResetPhotoData()
    {
        m_entry1.SetActive(false);
        m_entry2.SetActive(false);
        m_entry3.SetActive(false);
        m_photoAlbum.ResetPhotos();
        m_AmbienceToggle.StopMusic();
    }
}
