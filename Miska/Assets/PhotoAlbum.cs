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
    RawImage        m_photoOBJ;
    int             m_currPicIndex;

    // Start is called before the first frame update
    void Start()
    {
        m_folderPath = "/Photos";
        m_fullpath = Application.dataPath + m_folderPath;
        GetFileInfo();
        Debug.Log(m_fullpath);
        m_isShowingAlbum = false;
        m_currPicIndex = 0;
        m_photoOBJ = gameObject.GetComponentInChildren<RawImage>();
        m_photoOBJ.enabled = false;
    }

    private void GetFileInfo()
    {
        DirectoryInfo dir = new DirectoryInfo(m_fullpath);
        m_picsInfo = dir.GetFiles("*.png*");
    }

    // Update is called once per frame
    void Update()
    {
        m_photoOBJ.enabled = m_isShowingAlbum;
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (m_isShowingAlbum == false)
            {
                m_isShowingAlbum = true;
                StartCoroutine("ConvertToTexture", m_picsInfo[m_currPicIndex]);
                //ConvertToTexture(m_picsInfo[m_currPicIndex]);
            }
            if (m_isShowingAlbum == true)
            {
                m_isShowingAlbum = false;
            }
        }
    }

    IEnumerator ConvertToTexture(FileInfo filename)
    {
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(filename.Name))
        {
            yield return www;
            www.LoadImageIntoTexture(tex);
            GetComponent<Renderer>().material.mainTexture = tex;
        }
    }
}
