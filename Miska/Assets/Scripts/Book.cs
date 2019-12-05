using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles open/closing the player's journal as well as changing pages and animations
/// </summary>
public class Book : MonoBehaviour
{
    public Player m_player;
    public CameraController m_playerCamera;
    public Camera m_bookCamera;
    public GameObject m_pageMesh;
    public Transform m_pageTurnLeft; // the transform on the left side of the turning page
    public Transform m_pageTurnRight; // the transform on theright side of the turning page
    public Transform m_leftPages; // the transform that all leftpages are parented to
    public Transform m_rightPages; // the transform that all right pages are parented to
    public GameObject m_prevPageButton;
    public GameObject m_nextPageButton;
    public GameObject m_staticLeftPage; // the left page that is always active
    public GameObject m_staticRightPage; // the right page that is always active
    public Animation m_animation;
    public float m_openDelay;
    public bool m_autoClose;
    public bool m_manualControl = true;
    public bool m_menuBook;
    public AK.Wwise.Event m_stopPOIEvent;
    public PhotoMode m_photoMode;

    List<GameObject> m_pages;
    int m_currentPage;
    bool m_open;
    bool m_zoomed = false;
    float m_timer;

    Vector3 m_startPosition;
    Vector3 m_targetPosition;
    Quaternion m_startRotation;
    Quaternion m_targetRotation;

    bool m_opening;
    bool m_openDirection;
    bool m_startCalled;

    // Start is called before the first frame update
    public void Start()
    {
        if (!m_startCalled)
        {
            //m_pageTurn.clip.legacy = true;

            // Adds all the left and right pages in order to a list
            m_pages = new List<GameObject>();
            for (int i = 0; i < m_leftPages.childCount; i++)
            {
                m_pages.Add(m_leftPages.GetChild(i).gameObject);
                if (i < m_rightPages.childCount)
                    m_pages.Add(m_rightPages.GetChild(i).gameObject);
            }

            // Sets the world camera for all pages to the book camera
            Canvas[] pageCanvas = GetComponentsInChildren<Canvas>(true);
            foreach (Canvas canvas in pageCanvas)
            {
                canvas.worldCamera = m_bookCamera;
            }

            m_targetPosition = m_bookCamera.transform.localPosition;
            m_targetRotation = m_bookCamera.transform.localRotation;

            if (m_autoClose)
                CloseBook();
            //OpenBook();

            m_startCalled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_manualControl && Input.GetKeyDown(KeyCode.Tab) && !m_animation.isPlaying) // toggles the book
        {
            if (m_open)
            {
                if (!m_menuBook)
                {
                    CloseBook(); // closes the book
                }
                else
                {
                    m_opening = true;
                    m_open = false;
                    m_openDirection = false;
                    m_timer = 0;

                    // disables cursor
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;                    
                }
            }
            else if (!m_open)
            {
                OpenBook(0); // opens the book to the first page
            }
        }

        if (m_open) // handles turing the page when open
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) && !m_animation.isPlaying)
            {
                if (m_currentPage + 1 <= Mathf.CeilToInt(m_pages.Count / 2f) - 1) // checks if the next page exists
                    SetPage(m_currentPage + 1); // changes to the next page
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) && !m_animation.isPlaying)
            {
                if (m_currentPage - 1 >= 0) // if the previous page exists
                    SetPage(m_currentPage - 1); // changes to the previous page
            }            
        }

        if (m_opening) // lerps the camera downwards when opening
        {
            if (m_openDirection)
            {
                m_timer += Time.deltaTime;
                m_bookCamera.transform.localPosition = Vector3.Lerp(m_startPosition, m_targetPosition, m_timer / m_openDelay);
                m_bookCamera.transform.localRotation = Quaternion.Lerp(m_startRotation, m_targetRotation, m_timer / m_openDelay);

                if (m_timer >= m_openDelay)
                    m_opening = false;
            }
            else
            {
                m_timer += Time.deltaTime;
                m_bookCamera.transform.localPosition = Vector3.Lerp(m_targetPosition, m_startPosition, m_timer / m_openDelay);
                m_bookCamera.transform.localRotation = Quaternion.Lerp(m_targetRotation, m_startRotation, m_timer / m_openDelay);

                if (m_timer >= m_openDelay)
                {
                    m_opening = false;

                    m_bookCamera.enabled = false;
                    m_playerCamera.m_camera.enabled = true;
                    m_playerCamera.enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// opens the book to a specific page
    /// </summary>
    /// <param name="page">the page number to open to</param>
    public void OpenBook(int page)
    {
        if (!m_animation.isPlaying && !m_open) // if the book is not open
        {
            if (!m_menuBook) // only play the opening animation ingame
                m_animation.Play("Book_Open_001");
            else
                m_animation.Play("Book_Open_Menu_001");

            m_currentPage = page;
            m_leftPages.gameObject.SetActive(true);
            m_rightPages.gameObject.SetActive(true);


            // only show the next page button if there is a next page
            if (m_nextPageButton && m_currentPage + 1 <= Mathf.CeilToInt(m_pages.Count / 2f) - 1) 
                m_nextPageButton.SetActive(true);
            else if (m_nextPageButton)
                m_nextPageButton.SetActive(false);

            // only show the previous page button if there is a previous page
            if (m_prevPageButton && m_currentPage - 1 >= 0)
                m_prevPageButton.SetActive(true);
            else if (m_prevPageButton)
                m_prevPageButton.SetActive(false);

            // Sets the current pages to active
            m_pages[m_currentPage * 2].SetActive(true);
            m_pages[m_currentPage * 2 + 1].SetActive(true);

            if (m_player) // disables the player script while the book is open
                m_player.enabled = false;

            transform.GetChild(0).gameObject.SetActive(true); // enables the book mesh

            // enables the static pages
            m_staticLeftPage.SetActive(true); 
            m_staticRightPage.SetActive(true);

            m_open = true;
            // enable cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // change cameras
            m_bookCamera.enabled = true;
            m_playerCamera.m_camera.enabled = false;
            m_playerCamera.enabled = false;

            m_opening = true;
            m_openDirection = true;
            m_timer = 0;

            // sets the book camera to the player camera's position/rotation to lerp back down to the normal position
            m_bookCamera.transform.position = m_playerCamera.transform.position;
            m_bookCamera.transform.rotation = m_playerCamera.transform.rotation;

            m_startPosition = m_bookCamera.transform.localPosition;
            m_startRotation = m_bookCamera.transform.localRotation;
        }
    }

    /// <summary>
    /// Closes the book
    /// </summary>
    public void CloseBook()
    {
        if (m_player) // re-enables the player
            m_player.enabled = true;

        transform.GetChild(0).gameObject.SetActive(false); // disables the book mesh

        // disable pages
        m_staticLeftPage.SetActive(false);
        m_staticRightPage.SetActive(false);
        foreach (GameObject page in m_pages)
            page.SetActive(false);
        if (m_nextPageButton)
            m_nextPageButton.SetActive(false);
        if (m_prevPageButton)
            m_prevPageButton.SetActive(false);
        m_open = false;

        // disables cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_bookCamera.enabled = false;
        m_playerCamera.m_camera.enabled = true;
        m_playerCamera.enabled = true;

        if (m_zoomed) // if the player had zoomed into the map reset the position of the book
        {
            AnimationClip zoomReverse = m_animation.GetClip("Book_Zoom_Reverse_001");
            zoomReverse.SampleAnimation(gameObject, zoomReverse.length);
            m_zoomed = false;
        }

        m_opening = false;
        m_timer = 0;
    }

    /// <summary>
    /// Closes the book (for Photo Album)
    /// </summary>
    public void OnPhotoGallery()
    {
        transform.GetChild(0).gameObject.SetActive(false); // disables the book mesh

        // disable pages
        m_staticLeftPage.SetActive(false);
        m_staticRightPage.SetActive(false);
        foreach (GameObject page in m_pages)
            page.SetActive(false);
        if (m_nextPageButton)
            m_nextPageButton.SetActive(false);
        if (m_prevPageButton)
            m_prevPageButton.SetActive(false);
        m_open = false;

        if (m_zoomed) // if the player had zoomed into the map reset the position of the book
        {
            AnimationClip zoomReverse = m_animation.GetClip("Book_Zoom_Reverse_001");
            zoomReverse.SampleAnimation(gameObject, zoomReverse.length);
            m_zoomed = false;
        }
    }

    /// <summary>
    /// Toggle between zooming into the book
    /// </summary>
    public void LeftZoom()
    {
        if (!m_animation.isPlaying) // wait for previous animation to finsh
        {
            if (!m_zoomed)
            {
                m_animation.Play("Book_Zoom_001");
                m_zoomed = true;
                if (m_nextPageButton)
                    m_nextPageButton.SetActive(false);
                if (m_prevPageButton)
                    m_prevPageButton.SetActive(false);
            }
            else
            {
                m_animation.Play("Book_Zoom_Reverse_001");
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

    /// <summary>
    /// Changes the current page to the next in the list
    /// </summary>
    public void NextPage()
    {
        if (m_currentPage + 1 <= Mathf.CeilToInt(m_pages.Count / 2f) - 1) // makes sure there is another page
            SetPage(m_currentPage + 1);
    }

    /// <summary>
    /// changes the current page to the previous in the list
    /// </summary>
    public void PrevPage()
    {
        if (m_currentPage - 1 >= 0) // makes sure there is a previous page
            SetPage(m_currentPage - 1);
    }

    /// <summary>
    /// Starts the page change coroutine
    /// </summary>
    /// <param name="index">The page index</param>
    public void SetPage(int index)
    {
        if (index != m_currentPage) // can't change to the current page
            StartCoroutine(SetPageCoroutine(index, index > m_currentPage));
    }

    /// <summary>
    /// Handles turning the page 
    /// </summary>
    /// <param name="index">the page index</param>
    /// <param name="direction">the direction of the page turn true=right false=left</param>
    /// <returns></returns>
    IEnumerator SetPageCoroutine(int index, bool direction)
    {
        int oldPage = m_currentPage;
        m_currentPage = index;

        if (m_nextPageButton)
            m_nextPageButton.SetActive(false);
        if (m_prevPageButton)
            m_prevPageButton.SetActive(false);

        m_pageMesh.SetActive(true);

        if (direction) // right turn
        {
            // places the right side of the old page in the left side of the page turn
            m_pages[oldPage * 2 + 1].transform.parent = m_pageTurnLeft;
            m_pages[oldPage * 2 + 1].transform.localPosition = Vector3.zero;
            m_pages[oldPage * 2 + 1].transform.localRotation = Quaternion.identity;

            m_pages[m_currentPage * 2].SetActive(true); // enables the left side of the next page

            if ((m_currentPage * 2 + 1) < m_pages.Count) // enables the right side of the next page
                m_pages[m_currentPage * 2 + 1].SetActive(true);

            // places the left side of the next page in the right side of the page turn
            m_pages[m_currentPage * 2].transform.parent = m_pageTurnRight;
            m_pages[m_currentPage * 2].transform.localPosition = Vector3.zero;
            m_pages[m_currentPage * 2].transform.localRotation = Quaternion.identity;

            m_animation.Play("Book_Flip_001"); // plays the flip animation

            yield return WaitForAnimation(m_animation); // waits for the end of the flip animation

            // places the left/right pages back under the page heirachy 
            m_pages[oldPage * 2 + 1].transform.parent = m_rightPages;
            m_pages[oldPage * 2 + 1].transform.localPosition = Vector3.zero;
            m_pages[oldPage * 2 + 1].transform.localRotation = Quaternion.identity;
            m_pages[m_currentPage * 2].transform.parent = m_leftPages;
            m_pages[m_currentPage * 2].transform.localPosition = Vector3.zero;
            m_pages[m_currentPage * 2].transform.localRotation = Quaternion.identity;

            // disabels the old pages
            m_pages[oldPage * 2 + 1].SetActive(false);
            m_pages[oldPage * 2].SetActive(false);

        }
        else
        {
            // places the left side of the old page in the right side of the page turn
            m_pages[oldPage * 2].transform.parent = m_pageTurnRight;
            m_pages[oldPage * 2].transform.localPosition = Vector3.zero;
            m_pages[oldPage * 2].transform.localRotation = Quaternion.identity;

            // enables the next pages
            m_pages[m_currentPage * 2].SetActive(true);
            m_pages[m_currentPage * 2 + 1].SetActive(true);

            // places the right side of the next page into the left side of the page turn
            m_pages[m_currentPage * 2 + 1].transform.parent = m_pageTurnLeft;
            m_pages[m_currentPage * 2 + 1].transform.localPosition = Vector3.zero;
            m_pages[m_currentPage * 2 + 1].transform.localRotation = Quaternion.identity;

            m_animation.Play("Book_Flip_Reverse_001"); // play the reverse turn animation

            yield return WaitForAnimation(m_animation); // wait for the animation to finsh

            // places the left/right pages back under the page heirachy 
            m_pages[oldPage * 2].transform.parent = m_leftPages;
            m_pages[oldPage * 2].transform.localPosition = Vector3.zero;
            m_pages[oldPage * 2].transform.localRotation = Quaternion.identity;
            m_pages[m_currentPage * 2 + 1].transform.parent = m_rightPages;
            m_pages[m_currentPage * 2 + 1].transform.localPosition = Vector3.zero;
            m_pages[m_currentPage * 2 + 1].transform.localRotation = Quaternion.identity;
            
            // Disables the old pages
            if ((oldPage * 2 + 1) < m_pages.Count)            
                m_pages[oldPage * 2 + 1].SetActive(false);         
            
            m_pages[oldPage * 2].SetActive(false);
        }

        m_pageMesh.SetActive(false); // disable the page turn mesh

        // enables the next button if there is a next page
        if (m_nextPageButton && m_currentPage + 1 <= Mathf.CeilToInt(m_pages.Count / 2f) - 1)
            m_nextPageButton.SetActive(true);
        else if (m_nextPageButton)
            m_nextPageButton.SetActive(false);

        // enables the previous nutton if there is a previous page
        if (m_prevPageButton && m_currentPage - 1 >= 0)
            m_prevPageButton.SetActive(true);
        else if (m_prevPageButton)
            m_prevPageButton.SetActive(false);
    }
    
    /// <summary>
    /// Waits until there is no animation playing
    /// </summary>
    /// <param name="animation">the animation controller</param>
    /// <returns></returns>
    private IEnumerator WaitForAnimation(Animation animation)
    {
        do
        {
            yield return null;
        } while (animation.isPlaying);
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Retarts the current scene
    /// </summary>
    public void Restart()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        m_photoMode.ResetPhotoData();
        m_stopPOIEvent.Post(gameObject);
        SceneManager.LoadScene(0);
    }
  
}
 