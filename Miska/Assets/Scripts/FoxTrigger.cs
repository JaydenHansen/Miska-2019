using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enables an object when the player enters a trigger
/// </summary>
public class FoxTrigger : MonoBehaviour
{
    public GameObject m_fox;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            m_fox.SetActive(true);
    }
}
