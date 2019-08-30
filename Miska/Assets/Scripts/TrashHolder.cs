using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashHolder : MonoBehaviour
{
    int m_trashCount;
    public int TrashCount
    {
        get { return m_trashCount; }
        set { m_trashCount = value; }
    }

    public void OnTrashPickup()
    {
        m_trashCount++;        
    }
}
