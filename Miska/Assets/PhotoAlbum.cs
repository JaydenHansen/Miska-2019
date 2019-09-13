using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class PhotoAlbum : MonoBehaviour
{
    FileInfo[]      m_picsInfo;
    string          m_fullpath;
    string          m_folderPath;
    bool            m_isShowingAlbum;
    Canvas          m_canvas;
    RawImage        m_photoOBJ;
    int             m_currPicIndex;

    float           m_rawDimensionHeight = 950;

    public Player   m_playerScript;

    

    // Start is called before the first frame update
    void Start()
    {
        m_folderPath = "/Resources/Photos";
        m_fullpath = Application.dataPath + m_folderPath;
        GetFileInfo();
        m_isShowingAlbum = false;
        m_currPicIndex = 0;
        m_photoOBJ = gameObject.GetComponentInChildren<RawImage>();
        //m_photoOBJ.enabled = false;
        m_canvas = gameObject.GetComponentInChildren<Canvas>();
    }

    private void GetFileInfo()
    {
        DirectoryInfo dir = new DirectoryInfo(m_fullpath);
        m_picsInfo = dir.GetFiles("*.png");
    }

    // Update is called once per frame
    void Update()
    {
        m_canvas.enabled        = m_isShowingAlbum;
        m_playerScript.enabled  = !(m_isShowingAlbum);
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (m_isShowingAlbum == false)
            {
                m_isShowingAlbum = true;
                LoadIndexedPhotoToTexture();
            }
            else
            {
                m_isShowingAlbum = false;
            }
        }

        if (m_isShowingAlbum)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (m_currPicIndex == 0)
                {
                    m_currPicIndex = m_picsInfo.Length - 1;
                }
                else
                {
                    m_currPicIndex--;
                }
                LoadIndexedPhotoToTexture();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (m_currPicIndex == m_picsInfo.Length - 1)
                {
                    m_currPicIndex = 0;
                }
                else
                {
                    m_currPicIndex++;
                }
                LoadIndexedPhotoToTexture();
            }
        }
    }

    private void LoadIndexedPhotoToTexture()
    {
        string PhotoPath             = m_picsInfo[m_currPicIndex].Name;
        string loadPath                 = "Photos/" + Path.GetFileNameWithoutExtension(PhotoPath);
        var texture                            = Resources.Load<Texture2D>(loadPath);
        m_photoOBJ.texture      = texture;
    }
}
