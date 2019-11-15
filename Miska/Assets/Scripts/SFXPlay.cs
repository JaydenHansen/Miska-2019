using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple object that allows for script based Wwise Event triggering
/// </summary>
public class SFXPlay : MonoBehaviour
{
    public AK.Wwise.Event m_sound;

    public void PlaySound()
    {
        m_sound.Post(gameObject);
    }
}

