using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_UI : MonoBehaviour
{
    List<GameObject> m_checkboxes;
    List<bool> m_checkedStatus;
    GameObject m_GoalText;
    Animator m_animator;
    bool m_isAllChecked;
    int m_currentAreaTrash;
    public Texture2D m_box_empty, m_box_checked;

    // Start is called before the first frame update
    void Start()
    {
        SetupCheckboxArrays();
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //FOR TESTING ONLY DELETE WHEN UI TESTING IS COMPLETE
        if(Input.GetKeyDown(KeyCode.F))
        {
            m_animator.SetTrigger("TransTO_FullDetail");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            m_animator.SetTrigger("TransTO_Inactive");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            m_animator.SetTrigger("TransTO_PullBack");
        }
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

    public void SetupCheckboxes(int areaTrash)
    {
        for (int i = 0; i <= 5; i++)
        {
            if (i <= areaTrash)
            {
                m_checkboxes[i].SetActive(true);
            }
            else
            {
                m_checkboxes[i].SetActive(false);
            }
        }
    }

    public void CheckOffNextBox()
    {
        for (int i = 0; i < 5; i++)
        {
            if(m_checkedStatus[i] == false && !m_isAllChecked)
            {
                SetBoxChecked(i, true);
                if (i == m_currentAreaTrash - 1)
                {
                    StartCoroutine("AllTrashCollected");
                }
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

    IEnumerator AllTrashCollected()
    {
        //"List Complete" Animation (achievement + pull out checkboxes and disable)
        yield return new WaitForSeconds(0.5f);

        ResetArrayValues();
        //Change Goal Text
        m_animator.SetTrigger("TransTO_FullDetail");
        yield return null;
    }

    IEnumerator IntialTransition()
    {
        m_animator.SetTrigger("TransTO_FullDetail");
        yield return new WaitForSeconds(1.0f);

        m_animator.SetTrigger("TransTO_Pullback");
        yield return null;
    }
}
