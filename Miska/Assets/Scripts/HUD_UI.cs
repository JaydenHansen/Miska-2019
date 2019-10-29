using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_UI : MonoBehaviour
{
    List<GameObject> m_checkboxes;
    List<bool> m_checkedStatus;
    public GameObject m_GoalText;
    Animator m_animator;
    public bool m_isAllTrashPickedUp;
    int m_currentAreaTrash;
    public Texture2D m_box_empty, m_box_checked;
    public Texture2D m_gt_pickup3, m_gt_pickup3Done, m_gt_pickup4, m_gt_pickup4Done, m_gt_pickup5, m_gt_pickup5Done;

    Texture2D thisArea_gt_pickup, thisArea_gt_pickupDone;

    public Texture2D m_gt_dispose, m_gt_disposeDone;

    public AK.Wwise.Event m_checkOffSound, m_slideSound, m_stampSound;

    // Start is called before the first frame update
    void Start()
    {
        SetupCheckboxArrays();
        m_animator = GetComponent<Animator>();
        SetupTrashScene(3, 3);
        StartCoroutine("IntialTransition");
    }

    public void SetupTrashScene(int totalTrash, int remainingTrash)
    {
        SetupCheckboxes(totalTrash);
        m_currentAreaTrash = totalTrash;

        if (remainingTrash < totalTrash)
        {
            int disp = totalTrash - remainingTrash;
            for (int i = 0; i < disp; i++)
            {
                CheckOffNextBox();
            }
        }

        if (totalTrash == 3)
        {
            thisArea_gt_pickup = m_gt_pickup3;
            thisArea_gt_pickupDone = m_gt_pickup3Done;
        }
        else if (totalTrash == 4)
        {
            thisArea_gt_pickup = m_gt_pickup4;
            thisArea_gt_pickupDone = m_gt_pickup4Done;
        }
        else if (totalTrash == 5)
        {
            thisArea_gt_pickup = m_gt_pickup5;
            thisArea_gt_pickupDone = m_gt_pickup5Done;
        }
        RawImage ri = m_GoalText.GetComponent<RawImage>();
        ri.texture = thisArea_gt_pickup;
        StartCoroutine("IntialTransition");
    }

    public void OnTrashAreaExit()
    {
        m_animator.SetTrigger("TransTO_Inactive");
    }

    public void SetupDisposeScene()
    {
        SetupCheckboxes(0);
        RawImage ri = m_GoalText.GetComponent<RawImage>();
        ri.texture = m_gt_dispose;
        StartCoroutine("IntialTransition");
    }

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

    void ResetArrayValues()
    {
        for (int i = 0; i < 5; i++)
        {
            SetBoxChecked(i, false);
        }
    }

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

    public void CheckOffNextBox()
    {
        for (int i = 0; i < 5; i++)
        {
            if(m_checkedStatus[i] == false && !m_isAllTrashPickedUp)
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

    void SetBoxChecked(int boxNo, bool status)
    {
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

    void OnAllTrashCollected()
    {
        if (!m_isAllTrashPickedUp)
        {
            StartCoroutine("AllTrashCollected");
        }
    }

    public void OnAllTrashDisposed()
    {
        m_animator.SetTrigger("TransTO_Inactive");
    }

    public void OnTransition()
    {
        m_slideSound.Post(gameObject);
    }

    IEnumerator CheckOff(int i)
    {
        yield return new WaitForSeconds(0.8f);

        SetBoxChecked(i, true);
        m_checkOffSound.Post(gameObject);
    }

    IEnumerator AllTrashCollected()
    {
        m_isAllTrashPickedUp = true;
        m_animator.SetTrigger("TransTO_FullDetail");
        yield return new WaitForSeconds(2.0f);

        RawImage ri = m_GoalText.GetComponent<RawImage>();
        ri.texture = thisArea_gt_pickupDone;
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

    IEnumerator IntialTransition()
    {
        m_animator.SetTrigger("TransTO_FullDetail");
        yield return new WaitForSeconds(2.5f);

        m_animator.SetTrigger("TransTO_PullBack");
        yield return null;
    }
}
