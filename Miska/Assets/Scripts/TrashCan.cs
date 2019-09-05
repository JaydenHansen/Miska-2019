using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    public int m_requiredTrash;
    public TrashHolder m_trashCount;
    public GameObject[] m_trash;
    public VoidEvent m_onAllTrash;

    int m_trashLeft;
    public int TrashLeft
    {
        get { return m_trashLeft; }
        set { m_trashLeft = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_trashLeft = m_requiredTrash;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DepositTrash(Player player)
    {
        if (m_trashLeft > 0)
        {
            if (m_trashLeft >= m_trashCount.TrashCount)
            {
                Debug.Log("Deposited " + m_trashCount.TrashCount.ToString() + " trash");
                m_trashLeft -= m_trashCount.TrashCount;
                m_trashCount.TrashCount = 0;
            }
            else
            {
                Debug.Log("Deposited " + m_trashLeft.ToString() + " trash");
                m_trashCount.TrashCount -= m_trashLeft;
                m_trashLeft = 0;
            }

            if (m_trashLeft <= 0)
            {
                // do stuff
                Debug.Log("Collected all trash");
                m_onAllTrash.Invoke();
            }
        }
    }    
}
