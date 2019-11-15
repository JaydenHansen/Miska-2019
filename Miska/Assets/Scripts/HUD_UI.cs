using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Controls the UI overlay and associated animations and sounds
/// </summary>
public class HUD_UI : MonoBehaviour
{
    private     List<GameObject>    m_checkboxes;
    private     List<bool>          m_checkedStatus;
    private     Animator            m_animator;
    private     int                 m_currentAreaTrash;
    private     bool                m_isAllAreasCleared;
    private     GameObject          m_currentAreaBin;
    public      GameObject          m_GoalText;
    private     bool                m_isAllTrashPickedUp, m_isAllTrashDisposed;

    [Header("Images")]
    public      Texture2D           m_box_empty, m_box_checked;
    public      Texture2D           m_gt_pickup3, m_gt_pickup3Done, m_gt_pickup4, m_gt_pickup4Done, m_gt_pickup5, m_gt_pickup5Done;
    public      Texture2D           m_gt_dispose, m_gt_disposeDone;
    public      Texture2D           m_gt_returnToLodge;
    private     Texture2D           m_thisArea_gt_pickup, m_thisArea_gt_pickupDone;

    [Header("Audio Events")]
    public      AK.Wwise.Event      m_slideSound, m_stampSound;
    public      AK.Wwise.Event      m_stationPOIMusic, m_rocksPOIMusic, m_duckPOIMusic;
    private     AK.Wwise.Event      m_currentAreaMusic;
    
    [Header("Goal Refresh")]      
    bool m_isGoalRefreshing;
    public float m_goalRefreshTimeTotal;
    float m_goalRefreshTimeCurrent;

    

    

    // Start is called before the first frame update
    void Start()
    {
        SetupCheckboxArrays();
        m_animator = GetComponent<Animator>();
        m_isGoalRefreshing = false;
        m_currentAreaBin = GameObject.Find("BIN_Station");
        m_currentAreaMusic = m_stationPOIMusic;
        m_isAllAreasCleared = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GoalRefresh();
        }

        if (m_isGoalRefreshing)
        {
            m_goalRefreshTimeCurrent -= Time.deltaTime;
            Debug.Log(m_goalRefreshTimeCurrent.ToString());
            if (m_goalRefreshTimeCurrent <= 0.0f)
            {
                m_animator.SetTrigger("TransTO_PullBack");
                m_isGoalRefreshing = false;
            }
        }
    }


    /// <summary>
    /// Sets up scene for area with trash to collect
    /// </summary>
    /// <param name="binOBJ"> Game object for the area's bin</param>
    public void SetupTrashScene(GameObject binOBJ)
    {
        m_currentAreaMusic.Stop(m_currentAreaBin);

        TrashCan binScript = binOBJ.GetComponent<TrashCan>();

        int totalTrash = binScript.m_requiredTrash;
        int remainingTrash = binScript.TrashLeft;

        m_currentAreaBin = binOBJ;

        SetupCheckboxes(totalTrash);
        m_currentAreaTrash = totalTrash;

        //if some trash has been collected check off the checklist
        if (remainingTrash < totalTrash)
        {
            int disp = totalTrash - remainingTrash;
            for (int i = 0; i < disp; i++)
            {
                CheckOffNextBox();
            }
        }

        #region setting up goal text images
        if (totalTrash == 3)
        {
            m_thisArea_gt_pickup = m_gt_pickup3;
            m_thisArea_gt_pickupDone = m_gt_pickup3Done;
        }
        else if (totalTrash == 4)
        {
            m_thisArea_gt_pickup = m_gt_pickup4;
            m_thisArea_gt_pickupDone = m_gt_pickup4Done;
        }
        else if (totalTrash == 5)
        {
            m_thisArea_gt_pickup = m_gt_pickup5;
            m_thisArea_gt_pickupDone = m_gt_pickup5Done;
        }


        RawImage ri = m_GoalText.GetComponent<RawImage>();
        ri.texture = m_thisArea_gt_pickup;
        #endregion

        StartCoroutine("IntialTransition");
        string BinName = m_currentAreaBin.name;

        #region setting up music
        if (BinName == "BIN_Station")
        {
            m_currentAreaMusic = m_stationPOIMusic;
        }
        if (BinName == "BIN_Rocks")
        {
            m_currentAreaMusic = m_rocksPOIMusic;
        }
        if (BinName == "BIN_Ducks")
        {
            m_currentAreaMusic = m_duckPOIMusic;
        }
        #endregion

    }

    public void OnTrashAreaExit()
    {
        m_animator.SetTrigger("TransTO_Inactive");
    }

    /// <summary>
    /// Sets up scene for area where all trash has been collected
    /// </summary>
    public void SetupDisposeScene()
    {
        if(!m_isAllAreasCleared)
        {
            SetupCheckboxes(0);
            RawImage ri = m_GoalText.GetComponent<RawImage>();
            ri.texture = m_gt_dispose;
            StartCoroutine("IntialTransition");
        }
    }

    /// <summary>
    /// Sets up an array of checkboxes (triggers in start)
    /// </summary>
    private void SetupCheckboxArrays()
    {
        m_checkboxes = new List<GameObject>();
        m_checkedStatus = new List<bool>();

        for (int i = 1; i <= 5; i++)
        {
            GameObject go = GameObject.Find("Checkbox " + i.ToString());
            m_checkboxes.Add(go);
            m_checkedStatus.Add(false);
        }
    }

    /// <summary>
    /// Resets the values of checkbox arrays
    /// </summary>
    void ResetArrayValues()
    {
        for (int i = 0; i < 5; i++)
        {
            SetBoxChecked(i, false);
        }
        m_isAllTrashPickedUp = false;
    }

    /// <summary>
    /// Sets up number of checkboxes to display when entering a trash area
    /// </summary>
    /// <param name="areaTrash"> total number in trash for area</param>
    void SetupCheckboxes(int areaTrash)
    {
        ResetArrayValues();
        for (int i = 0; i < 5; i++)
        {
            if (i < areaTrash)
            {
                m_checkboxes[i].gameObject.GetComponent<RawImage>().enabled = true;
            }
            else
            {
                m_checkboxes[i].gameObject.GetComponent<RawImage>().enabled = false;
            }
        }
    }

    /// <summary>
    /// Checks off next box in list, checks for all trash in area being collected
    /// </summary>
    public void CheckOffNextBox()
    {
        for (int i = 0; i < 5; i++)
        {
            if (m_checkedStatus[i] == false && !m_isAllTrashPickedUp)
            {
                StartCoroutine("CheckOff", i);
                if (i + 1 == m_currentAreaTrash)
                {
                    OnAllTrashCollected();
                }
                break;
            }
        }
    }


    /// <summary>
    /// Sets the checked status (checked/unchecked) of a specific box in checklist
    /// </summary>
    /// <param name="boxNo">Box as location in array</param>
    /// <param name="status">box set to checked/unchecked</param>
    void SetBoxChecked(int boxNo, bool status)
    {
        m_isAllTrashDisposed = false;
        m_isAllTrashPickedUp = false;
        m_checkedStatus[boxNo] = status;
        RawImage ri = m_checkboxes[boxNo].GetComponent<RawImage>();
        if (status)
        {
            ri.texture = m_box_checked;
        }
        else
        {
            ri.texture = m_box_empty;
        }
    }

    /// <summary>
    /// Runs animation upon all trash being collected
    /// </summary>
    void OnAllTrashCollected()
    {
        if (!m_isAllTrashPickedUp)
        {
            StartCoroutine("AllTrashCollected");
        }
    }

    /// <summary>
    /// Run animation upon all trash being disposed
    /// </summary>
    public void OnAllTrashDisposed()
    {
        if (!m_isAllTrashDisposed)
        {
            StartCoroutine(AllTrashDisposed(m_currentAreaMusic, m_currentAreaBin));
        }
    }

    /// <summary>
    /// Plays sound during transition
    /// </summary>
    public void OnTransition()
    {
        m_slideSound.Post(gameObject);
    }

    /// <summary>
    /// Checks off box after a delay
    /// </summary>
    /// <param name="i">box number</param>
    /// <returns></returns>
    IEnumerator CheckOff(int i)
    {
        yield return new WaitForSeconds(0.8f);

        SetBoxChecked(i, true);
        //m_checkOffSound.Post(gameObject);
    }


    /// <summary>
    /// Animation for all trash collected
    /// </summary>
    IEnumerator AllTrashCollected()
    {
        m_isAllTrashPickedUp = true;
        m_animator.SetTrigger("TransTO_FullDetail");
        yield return new WaitForSeconds(2.0f);

        RawImage ri = m_GoalText.GetComponent<RawImage>();
        ri.texture = m_thisArea_gt_pickupDone;
        m_stampSound.Post(gameObject);
        yield return new WaitForSeconds(1.5f);


        m_animator.SetTrigger("TransTO_Inactive");
        yield return new WaitForSeconds(1.5f);

        SetupCheckboxes(0);
        ResetArrayValues();
        ri.texture = m_gt_dispose;
        m_animator.SetTrigger("TransTO_FullDetail");
        yield return null;
    }

    /// <summary>
    /// Animation for the trash disposed
    /// </summary>
    /// <param name="music"> This Area's music </param>
    /// <param name="bin">This Area's bin</param>
    /// <returns></returns>
    IEnumerator AllTrashDisposed(AK.Wwise.Event music, GameObject bin)
    {
        yield return new WaitForSeconds(0.4f);

        m_isAllTrashDisposed = true;
        music.Post(bin);
        yield return new WaitForSeconds(0.7f);

        RawImage ri = m_GoalText.GetComponent<RawImage>();
        ri.texture = m_gt_disposeDone;
        yield return new WaitForSeconds(1.5f);

        m_animator.SetTrigger("TransTO_Inactive");
        if (m_isAllAreasCleared)
        {
            yield return new WaitForSeconds(0.3f);

            ri.texture = m_gt_returnToLodge;
            m_animator.SetTrigger("TransTO_FullDetail");
        }

        yield return null;
    }

    /// <summary>
    /// Transition in from the start of an area
    /// </summary>
    IEnumerator IntialTransition()
    {
        m_animator.SetTrigger("TransTO_FullDetail");
        yield return new WaitForSeconds(3.5f);

        m_animator.SetTrigger("TransTO_PullBack");
        yield return null;
    }

    /// <summary>
    /// "Refreshes" the goal informaiton
    /// </summary>
    void GoalRefresh()
    {
        if (!m_isGoalRefreshing)
        {
            m_animator.SetTrigger("TransTO_FullDetail");
        }
        m_goalRefreshTimeCurrent = m_goalRefreshTimeTotal;
        m_isGoalRefreshing = true;
    }

    /// <summary>
    /// Sets up the Return to Lodge 
    /// </summary>
    public void SetupReturnToLodge()
    {
        m_isAllAreasCleared = true;
    }
}
