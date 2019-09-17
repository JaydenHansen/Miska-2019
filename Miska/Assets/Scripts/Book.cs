using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Book : MonoBehaviour
{
    public Player m_player;
    public CameraController m_playerCamera;
    public Camera m_bookCamera;
    public Camera m_groundCamera;
    public GameObject m_pageMesh;
    public Transform m_pageTurnLeft;
    public Transform m_pageTurnRight;
    public Transform m_leftPages;
    public Transform m_rightPages;
    public GameObject m_prevPageButton;
    public GameObject m_nextPageButton;
    public GameObject m_staticLeftPage;
    public GameObject m_staticRightPage;
    public Animation m_pageTurn;
    public float m_openDelay;

    List<GameObject> m_pages;
    int m_currentPage;
    bool m_open;
    Vector3 m_baseCameraPos;
    bool m_zoomed = false;
    float m_timer;

    Vector3 m_startPosition;
    Vector3 m_targetPosition;
    Quaternion m_startRotation;
    Quaternion m_targetRotation;

    bool TMP;

    Dictionary<AreaName, bool> m_checklist;
    public ProgressLog m_progress;

    // Start is called before the first frame update
    void Start()
    {
        //m_pageTurn.clip.legacy = true;

        m_checklist = new Dictionary<AreaName, bool>();

        m_checklist.Add(AreaName.STATION, false);
        m_checklist.Add(AreaName.ROCKS, false);
        m_checklist.Add(AreaName.DUCKS, false);

        m_pages = new List<GameObject>();
        for(int i = 0; i < m_leftPages.childCount; i++)
        {
            m_pages.Add(m_leftPages.GetChild(i).gameObject);
            if (i < m_rightPages.childCount)
                m_pages.Add(m_rightPages.GetChild(i).gameObject);
        }

        Canvas[] pageCanvas = GetComponentsInChildren<Canvas>(true);
        foreach(Canvas canvas in pageCanvas)
        {
            canvas.worldCamera = m_bookCamera;
        }

        m_targetPosition = m_bookCamera.transform.localPosition;
        m_targetRotation = m_bookCamera.transform.localRotation;

        CloseBook();
        //OpenBook();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !m_pageTurn.isPlaying)
        {
            if (m_open)
            {
                CloseBook();
            }
            else if (!m_open)
            {
                OpenBook();
            }
        }
        if (m_open)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) && !m_pageTurn.isPlaying)
            {
                if (m_currentPage + 1 <= Mathf.CeilToInt(m_pages.Count / 2f) - 1)
                    SetPage(m_currentPage + 1);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) && !m_pageTurn.isPlaying)
            {
                if (m_currentPage - 1 >= 0)
                    SetPage(m_currentPage - 1);
            }            
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                m_bookCamera.transform.localPosition = m_baseCameraPos;
            }
        }

        if (TMP)
        {
            m_timer += Time.deltaTime;
            m_bookCamera.transform.localPosition = Vector3.Lerp(m_startPosition, m_targetPosition, m_timer / m_openDelay);
            m_bookCamera.transform.localRotation = Quaternion.Lerp(m_startRotation, m_targetRotation, m_timer / m_openDelay);
        }
    }

    public void OpenBook()
    {
        if (!m_pageTurn.isPlaying)
        {
            m_currentPage = 0;
            m_leftPages.gameObject.SetActive(true);
            m_rightPages.gameObject.SetActive(true);
            if (m_nextPageButton)
                m_nextPageButton.SetActive(true);
            m_pages[m_currentPage * 2].SetActive(true);
            m_pages[m_currentPage * 2 + 1].SetActive(true);

            if (m_player)
                m_player.enabled = false;
            transform.GetChild(0).gameObject.SetActive(true);
            m_staticLeftPage.SetActive(true);
            m_staticRightPage.SetActive(true);
            m_open = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            m_bookCamera.enabled = true;
            m_groundCamera.enabled = true;
            m_playerCamera.m_camera.enabled = false;
            m_playerCamera.enabled = false;

            TMP = true;            

            m_bookCamera.transform.position = m_playerCamera.transform.position;
            m_bookCamera.transform.rotation = m_playerCamera.transform.rotation;

            m_startPosition = m_bookCamera.transform.localPosition;
            m_startRotation = m_bookCamera.transform.localRotation;
        }
    }

    public void CloseBook()
    {
        if (m_player)
            m_player.enabled = true;
        transform.GetChild(0).gameObject.SetActive(false);
        m_staticLeftPage.SetActive(false);
        m_staticRightPage.SetActive(false);
        foreach (GameObject page in m_pages)
            page.SetActive(false);
        if (m_nextPageButton)
            m_nextPageButton.SetActive(false);
        if (m_prevPageButton)
            m_prevPageButton.SetActive(false);
        m_open = false;
        Cursor.lockState = CursorLockMode.Locked;

        m_bookCamera.enabled = false;
        m_groundCamera.enabled = false;
        m_playerCamera.m_camera.enabled = true;
        m_playerCamera.enabled = true;

        if (m_zoomed)
        {
            AnimationClip zoomReverse = m_pageTurn.GetClip("Book_Zoom_Reverse_001");
            zoomReverse.SampleAnimation(gameObject, zoomReverse.length);
            m_zoomed = false;
        }
        TMP = false;
        m_timer = 0;
    }

    public void LeftZoom()
    {
        if (!m_pageTurn.isPlaying)
        {
            if (!m_zoomed)
            {
                m_pageTurn.Play("Book_Zoom_001");
                m_zoomed = true;
                if (m_nextPageButton)
                    m_nextPageButton.SetActive(false);
                if (m_prevPageButton)
                    m_prevPageButton.SetActive(false);
            }
            else
            {
                m_pageTurn.Play("Book_Zoom_Reverse_001");
                m_zoomed = false;
                if (m_nextPageButton && m_currentPage + 1 <= Mathf.CeilToInt(m_pages.Count / 2f) - 1)
                    m_nextPageButton.SetActive(true);
                else if (m_nextPageButton)
                    m_nextPageButton.SetActive(false);

                if (m_prevPageButton && m_currentPage - 1 >= 0)
                    m_prevPageButton.SetActive(true);
                else if (m_prevPageButton)
                    m_prevPageButton.SetActive(false);
            }
        }
    }

    public void NextPage()
    {
        if (m_currentPage + 1 <= Mathf.CeilToInt(m_pages.Count / 2f) - 1)
            SetPage(m_currentPage + 1);
    }

    public void PrevPage()
    {
        if (m_currentPage - 1 >= 0)
            SetPage(m_currentPage - 1);
    }

    public void SetPage(int index)
    {
        if (index != m_currentPage)
            StartCoroutine(SetPageCoroutine(index, index > m_currentPage));
    }

    IEnumerator SetPageCoroutine(int index, bool direction)
    {
        int oldPage = m_currentPage;
        m_currentPage = index;

        if (m_nextPageButton)
            m_nextPageButton.SetActive(false);
        if (m_prevPageButton)
            m_prevPageButton.SetActive(false);

        m_pageMesh.SetActive(true);

        if (direction)
        {
            m_pages[oldPage * 2 + 1].transform.parent = m_pageTurnLeft;
            m_pages[oldPage * 2 + 1].transform.localPosition = Vector3.zero;
            m_pages[oldPage * 2 + 1].transform.localRotation = Quaternion.identity;

            m_pages[m_currentPage * 2].SetActive(true);

            if ((m_currentPage * 2 + 1) < m_pages.Count)
                m_pages[m_currentPage * 2 + 1].SetActive(true);

            m_pages[m_currentPage * 2].transform.parent = m_pageTurnRight;
            m_pages[m_currentPage * 2].transform.localPosition = Vector3.zero;
            m_pages[m_currentPage * 2].transform.localRotation = Quaternion.identity;

            m_pageTurn.Play("Book_Flip_001");

            yield return WaitForAnimation(m_pageTurn);

            m_pages[oldPage * 2 + 1].transform.parent = m_rightPages;
            m_pages[oldPage * 2 + 1].transform.localPosition = Vector3.zero;
            m_pages[oldPage * 2 + 1].transform.localRotation = Quaternion.identity;

            m_pages[oldPage * 2 + 1].SetActive(false);
            m_pages[oldPage * 2].SetActive(false);

            m_pages[m_currentPage * 2].transform.parent = m_leftPages;
            m_pages[m_currentPage * 2].transform.localPosition = Vector3.zero;
            m_pages[m_currentPage * 2].transform.localRotation = Quaternion.identity;
        }
        else
        {
            m_pages[oldPage * 2].transform.parent = m_pageTurnRight;
            m_pages[oldPage * 2].transform.localPosition = Vector3.zero;
            m_pages[oldPage * 2].transform.localRotation = Quaternion.identity;

            m_pages[m_currentPage * 2].SetActive(true);
            m_pages[m_currentPage * 2 + 1].SetActive(true);

            m_pages[m_currentPage * 2 + 1].transform.parent = m_pageTurnLeft;
            m_pages[m_currentPage * 2 + 1].transform.localPosition = Vector3.zero;
            m_pages[m_currentPage * 2 + 1].transform.localRotation = Quaternion.identity;

            m_pageTurn.Play("Book_Flip_Reverse_001");

            yield return WaitForAnimation(m_pageTurn);
          
            m_pages[oldPage * 2].transform.parent = m_leftPages;
            m_pages[oldPage * 2].transform.localPosition = Vector3.zero;
            m_pages[oldPage * 2].transform.localRotation = Quaternion.identity;

            if ((oldPage * 2 + 1) < m_pages.Count)            
                m_pages[oldPage * 2 + 1].SetActive(false);         
            
            m_pages[oldPage * 2].SetActive(false);

            m_pages[m_currentPage * 2 + 1].transform.parent = m_rightPages;
            m_pages[m_currentPage * 2 + 1].transform.localPosition = Vector3.zero;
            m_pages[m_currentPage * 2 + 1].transform.localRotation = Quaternion.identity;
        }

        m_pageMesh.SetActive(false);

        if (m_nextPageButton && m_currentPage + 1 <= Mathf.CeilToInt(m_pages.Count / 2f) - 1)
            m_nextPageButton.SetActive(true);
        else if (m_nextPageButton)
            m_nextPageButton.SetActive(false);

        if (m_prevPageButton && m_currentPage - 1 >= 0)
            m_prevPageButton.SetActive(true);
        else if (m_prevPageButton)
            m_prevPageButton.SetActive(false);
    }

    //Updates Checklist
    void updateChecklist()
    {
        bool[] prog = m_progress.getProgressChecks();
        for (int i = 0; i < 3; i++)                             //Iterate through lists
        {
            bool prog_bl = prog[i];
            bool list_bl = m_checklist[(AreaName)i];
            if (prog_bl != list_bl)                             //if there is a discrepancy between the progress, and that recorded on the checklist
            {
                m_checklist[(AreaName)i] = prog_bl;             //set checklist item to match the progress log
                //start animation
            }
        }
    }
    
    private IEnumerator WaitForAnimation(Animation animation)
    {
        do
        {
            yield return null;
        } while (animation.isPlaying);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
  
}
 