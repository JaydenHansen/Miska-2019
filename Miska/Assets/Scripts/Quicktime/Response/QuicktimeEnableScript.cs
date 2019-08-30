using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicktimeEnableScript : QuicktimeResponse
{
    public MonoBehaviour m_script;

    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {        
    }

    public override void OnSuccess()
    {
        m_script.enabled = true;
    }

    public override void OnFailure()
    {
        m_script.enabled = false;
    }
}
