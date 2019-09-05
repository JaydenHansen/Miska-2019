using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicktimeDestroyObject : QuicktimeResponse
{
    public override void OnSuccess()
    {
        Destroy(gameObject);
    }
}
