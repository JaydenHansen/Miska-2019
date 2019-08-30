using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlay : MonoBehaviour
{
    AudioSource m_sound;

    // Start is called before the first frame update
    void Start()
    {
        m_sound = GetComponent<AudioSource>();
    }

public void PlaySound()
    {
        m_sound.Play();
    }
}
