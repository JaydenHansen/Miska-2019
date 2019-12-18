using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


/// <summary>
/// Displays the Captured Images in the Gallery and Journal Entries
/// </summary>
public class PhotoAlbum : MonoBehaviour
{
    
    private     DirectoryInfo       m_photoDirectory;
    /// <summary>
    /// Array of all pictures in fullpath folder
    /// </summary>
    private     List<FileInfo>      m_photoRollFileInfo;

    private     bool                m_isShowingAlbum;
    /// <summary>
    /// Object that displays images, texture changes to show new images
    /// </summary>
    public     RawImage            m_photoOBJ;
    /// <summary>
    /// current location in picture index
    /// </summary>
    private     int                 m_currPicIndex;
    private     List<PhotoSubject>  m_journalEntries;

    /// <summary>
    /// The photo canvas, incl. BG, Text & Buttons
    /// </summary>
    public      Canvas              m_canvas;
    public      Player              m_playerScript;
    public      Camera              m_virtCam;
    public      CameraController    m_camControl;

    // Start is called before the first frame update
    void Start()
    {
        m_photoDirectory = Directory.CreateDirectory(Application.persistentDataPath + "/Photos");
        m_isShowingAlbum = false;
        m_currPicIndex = 0;
        FileInfo[] m_picFileInfo = m_photoDirectory.GetFiles("*.png");
        m_photoRollFileInfo = new List<FileInfo>();
        PhotoMode phm = GameObject.Find("Screen Capture").GetComponent<PhotoMode>();
        foreach (var pic in m_picFileInfo)
        {
            if (pic.Name.Contains("RIV"))
            {
                phm.LoadInJournalEntry(JournalSubject.LOC_River, pic);
            }
            else if (pic.Name.Contains("FLY"))
            {
                phm.LoadInJournalEntry(JournalSubject.LOC_Fireflies, pic);
            }
            else if (pic.Name.Contains("DCK"))
            {
                phm.LoadInJournalEntry(JournalSubject.LOC_DuckPond, pic);
            }
            else
            {
                m_photoRollFileInfo.Add(pic);
            }

        }
        m_journalEntries = new List<PhotoSubject>();
    }

    public void ResetPhotos()
    {
        FileInfo[] m_picFileInfo = m_photoDirectory.GetFiles("*.png");
        foreach (var file in m_picFileInfo)
        {
            File.Delete(file.FullName);
        }
        Directory.Delete(m_photoDirectory.FullName);
        m_photoDirectory = Directory.CreateDirectory(Application.persistentDataPath + "/Photos");
        m_isShowingAlbum = false;
        m_currPicIndex = 0;
        m_photoRollFileInfo = new List<FileInfo>();
    }


    /// <summary>
    /// Toggles the album visibility
    /// </summary>
    public void toggleShowingAlbum()
    {
        setShowingAlbum(!m_isShowingAlbum);
    }


    /// <summary>
    /// Sets the showing album state
    /// </summary>
    /// <param name="status">show/hide album</param>
    void setShowingAlbum(bool status)
    {
        m_isShowingAlbum = status;
        m_playerScript.enabled = !status;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        m_camControl.m_camera.enabled = !status;

        GameObject.Find("CameraArmMain").GetComponent<CameraController>().enabled = !(status);
        m_virtCam.enabled = status;
        if (status && m_photoRollFileInfo.Count != 0) { LoadIndexedPhotoToTexture(); }
    }

    // Update is called once per frame
    void Update()
    {
        m_canvas.enabled        = m_isShowingAlbum;
        
        //if left is pressed, current pic moves "back". right moves it "forward". Wraps from start to finish. Then loads image
        if(Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            PrevPhoto();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextPhoto();
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && m_isShowingAlbum)
        {
            setShowingAlbum(false);
        }
    }

    /// <summary>
    /// Loads currently selected photo in array
    /// </summary>
    private void LoadIndexedPhotoToTexture()
    {

        var PhotoInfo = m_photoRollFileInfo[m_currPicIndex];
        var texture = LoadPNG(PhotoInfo);
        if(!m_photoOBJ) { Debug.Log("photo Object not found"); }
        m_photoOBJ.texture = texture;

    }

    /// <summary>
    /// Loads a texture from .png file
    /// </summary>
    /// <param name="filePath"> path of image to load </param>
    /// <returns> image found at filepath </returns>
    Texture2D LoadPNG(FileInfo photoInfo)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (photoInfo.Exists)
        {
            string fn = photoInfo.FullName;
            fileData = File.ReadAllBytes(fn);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    /// <summary>
    /// Adds a new path to array that holds a photo
    /// </summary>
    /// <param name="filename">photo's filepath</param>
    public void AddNewPhoto(string file)
    {
        FileInfo newPic = new FileInfo(file);

        m_photoRollFileInfo.Add(newPic);
        if(m_photoRollFileInfo.Count == 1)
        {
            LoadIndexedPhotoToTexture();
        }
    }

    public void NextPhoto()
    {
        if (m_currPicIndex == m_photoRollFileInfo.Count - 1)
        {
            m_currPicIndex = 0;
        }
        else
        {
            m_currPicIndex++;
        }
        LoadIndexedPhotoToTexture();
    }

    public void PrevPhoto()
    {
        if (m_currPicIndex == 0)
        {
            m_currPicIndex = m_photoRollFileInfo.Count - 1;
        }
        else
        {
            m_currPicIndex--;
        }
        LoadIndexedPhotoToTexture();
    }
}
