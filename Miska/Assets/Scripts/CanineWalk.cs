using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanineWalk : MonoBehaviour
{
    public AK.Wwise.Event m_canineStep;

    public void PlayStepSound()
    {
        m_canineStep.Post(gameObject);
    }

}
