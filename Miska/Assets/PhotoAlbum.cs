using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class PhotoAlbum : MonoBehaviour
{

    List<string>m_picsFileNames;

    Dictionary<JournalPhoto, string> m_JournalPhotos;

    string          m_fullpath;
    string          m_folderPath;
    bool            m_isShowingAlbum;
    public Canvas          m_canvas;
    RawImage        m_photoOBJ;
    int             m_currPicIndex;

    public Player   m_playerScript;

    

    // Start is called before the first frame update
    void Start()
    {
        m_picsFileNames = new List<string>();
        m_folderPath = "/Resources/Photos";
        m_fullpath = Application.dataPath + m_folderPath;
        m_isShowingAlbum = false;
        m_currPicIndex = 0;
        GetFileArray();
        m_photoOBJ = gameObject.GetComponentInChildren<RawImage>();
        //m_photoOBJ.enabled = false;
        
    }

    private void GetFileArray()
    {
        DirectoryInfo dir = new DirectoryInfo(m_fullpath);
        var picsInfo = dir.GetFiles("*.png");
        foreach (var file in picsInfo)
        {
            m_picsFileNames. Add(file.Name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_canvas.enabled        = m_isShowingAlbum;
        //m_playerScript.enabled  = !(m_isShowingAlbum);
        /*if (Input.GetKeyDown(KeyCode.L))
        {
            if (m_isShowingAlbum == false)
            {
                m_isShowingAlbum = true;
                m_playerScript.enabled = false;
                LoadIndexedPhotoToTexture();
            }
            else
            {
                m_isShowingAlbum = false;
                m_playerScript.enabled = true;
            }
        }*/

        /*
        if (m_isShowingAlbum)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
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
            if (Input.GetKeyDown(KeyCode.RightArrow))
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
        }*/
    }

    private void LoadIndexedPhotoToTexture()
    {

        string PhotoPath = m_picsFileNames[m_currPicIndex];
        string loadPath = m_fullpath + "/" + PhotoPath;
        var texture = LoadPNG(loadPath);
        m_photoOBJ.texture = texture;

    }

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

    public void AddNewPhoto(string filename)
    {
        m_picsFileNames.Add(filename);
    }

    public void AddToJournal(string filename, JournalPhoto photoPlace)
    {
        m_JournalPhotos.Add(photoPlace, filename);
    }
}
