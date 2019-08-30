using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicktimeAddToChildren : QuicktimeResponse
{
    public Transform m_parent;
    public Transform m_child;

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
        m_child.parent = m_parent;
        m_child.localPosition = Vector3.zero;
    }
}
