﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class PhotoAlbum : MonoBehaviour
{

    List<string>        m_picsFileNames;

    string              m_fullpath;
    string              m_folderPath;
    bool                m_isShowingAlbum;
    public Canvas              m_canvas;
    RawImage            m_photoOBJ;
    int                 m_currPicIndex;

    public Player       m_playerScript;

    List<PhotoSubject>  m_journalEntries;

    

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
        m_journalEntries = new List<PhotoSubject>();
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

    public void toggleShowingAlbum()
    {
        setShowingAlbum(!m_isShowingAlbum);
    }

    void setShowingAlbum(bool status)
    {
        m_isShowingAlbum = status;
        m_playerScript.enabled = !(status);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameObject.Find("CameraArmMain").GetComponent<Camera>().enabled = !(status);
        GameObject.Find("VirtCamera").SetActive(status);
    }

    // Update is called once per frame
    void Update()
    {
        m_canvas.enabled        = m_isShowingAlbum;
        
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
}
