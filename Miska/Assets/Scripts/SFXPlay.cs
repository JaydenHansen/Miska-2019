using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SFXPlay : MonoBehaviour
{
    public AK.Wwise.Event m_sound;

    public void PlaySound()
    {
        m_sound.Post(gameObject);
    }
}

