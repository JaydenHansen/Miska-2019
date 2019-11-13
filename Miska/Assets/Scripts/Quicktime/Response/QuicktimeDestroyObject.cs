using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys the attached gameobject on success
/// </summary>
public class QuicktimeDestroyObject : QuicktimeResponse
{
    public override void OnSuccess()
    {
        Destroy(gameObject);
    }
}
