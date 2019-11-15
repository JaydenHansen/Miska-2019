using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if the player has player has picked up all the trash in the area and calls an event
/// </summary>
public class TrashCan : MonoBehaviour
{
    public int m_requiredTrash;
    public GameObject[] m_trash;
    public VoidEvent m_onAllTrash;
    public VoidEvent m_trashDeposited;

    public AK.Wwise.Event m_depositTrashSound;
    public AK.Wwise.Event m_allTrashDepositSound;

    int m_trashLeft;
    bool m_triggered;
    Renderer m_renderer;
    public int TrashLeft
    {
        get { return m_trashLeft; }
        set { m_trashLeft = value; }
    }
    public bool Triggered
    {
        get { return m_triggered; }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_trashLeft = m_requiredTrash;
        m_renderer = GetComponent<Renderer>();
    }

    /// <summary>
    /// Checks if there is no trash left and calls the event
    /// </summary>
    public void DepositTrash()
    {        
        if (m_trashLeft <= 0 && !m_triggered)
        {
            m_triggered = true;
            m_onAllTrash.Invoke();
            m_trashDeposited.Invoke();
        }
        else
        {
            m_trashDeposited.Invoke();
        }
    }   

    /// <summary>
    /// Called when trash has been picked up and counts down how much trash is left
    /// </summary>
    public void OnTrashPickup()
    {
        m_trashLeft--;
    }
}
