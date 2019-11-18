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
    
    private     string              m_fullpath;
    /// <summary>
    /// Array of all pictures in fullpath folder
    /// </summary>
    private     List<string>        m_picsFileNames;

    private     bool                m_isShowingAlbum;
    /// <summary>
    /// Object that displays images, texture changes to show new images
    /// </summary>
    private     RawImage            m_photoOBJ;
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

    // Start is called before the first frame update
    void Start()
    {
        m_picsFileNames = new List<string>();
        m_fullpath = Application.dataPath + "/Photos";
        m_isShowingAlbum = false;
        m_currPicIndex = 0;
        GetFileArray();
        m_photoOBJ = gameObject.GetComponentInChildren<RawImage>();
        m_journalEntries = new List<PhotoSubject>();
        Debug.Log("start not skipped");
    }

    /// <summary>
    /// Gets array of .png files from folder
    /// </summary>
    private void GetFileArray()
    {
        DirectoryInfo dir = new DirectoryInfo(m_fullpath);
        var picsInfo = dir.GetFiles("*.png");
        foreach (var file in picsInfo)
        {
            m_picsFileNames. Add(file.Name);
        }
        if (m_picsFileNames.Count != 0)
        {
            LoadIndexedPhotoToTexture();
        }
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
        m_playerScript.enabled = !(status);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameObject.Find("CameraArmMain").GetComponent<CameraController>().enabled = !(status);
        m_virtCam.enabled = status;
        if (status) { LoadIndexedPhotoToTexture(); }
    }

    // Update is called once per frame
    void Update()
    {
        m_canvas.enabled        = m_isShowingAlbum;
        
        //if left is pressed, current pic moves "back". right moves it "forward". Wraps from start to finish. Then loads image
        if(Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            if (m_currPicIndex == 0)
            {
                m_currPicIndex = m_picsFileNames.Count - 1;
            }
            else
            {
                m_currPicIndex--;
            }
            LoadIndexedPhotoToTexture();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (m_currPicIndex == m_picsFileNames.Count - 1)
            {
                m_currPicIndex = 0;
            }
            else
            {
                m_currPicIndex++;
            }
            LoadIndexedPhotoToTexture();
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

        string PhotoPath = m_picsFileNames[m_currPicIndex];
        string loadPath = m_fullpath + "/" + PhotoPath;
        var texture = LoadPNG(loadPath);
        m_photoOBJ.texture = texture;

    }

    /// <summary>
    /// Loads a texture from .png file
    /// </summary>
    /// <param name="filePath"> path of image to load </param>
    /// <returns> image found at filepath </returns>
    Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    /// <summary>
    /// Adds a new path to array that holds a photo
    /// </summary>
    /// <param name="filename">photo's filepath</param>
    public void AddNewPhoto(string filename)
    {
        m_picsFileNames.Add(filename);
        if(m_picsFileNames.Count == 1)
        {
            LoadIndexedPhotoToTexture();
        }
    }
}
