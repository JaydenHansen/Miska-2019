using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The gate that stops the player from ending before all POIs are complete
/// </summary>
public class Gate : MonoBehaviour
{
    public int m_trashCanCount;
    public Transform m_openPosition;
    public VoidEvent m_allAreasComplete;

    /// <summary>
    /// Called each time an area is completed
    /// opens that gate when there are no trashcans left
    /// </summary>
    public void AreaComplete()
    {
        m_trashCanCount--;
        if (m_trashCanCount == 0) // if there are no trash cans left
        {
            // moves the gate to the open position
            transform.position = m_openPosition.position;
            transform.rotation = m_openPosition.rotation;
          
            GameObject.Find("Goal UI").GetComponent<HUD_UI>().SetupReturnToLodge();

            m_allAreasComplete.Invoke();
        }
    }
}
