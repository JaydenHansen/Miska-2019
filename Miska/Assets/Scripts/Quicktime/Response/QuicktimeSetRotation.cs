using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicktimeSetRotation : QuicktimeResponse
{
    public Transform m_transform;
    public Vector3 m_direction;
    public bool m_localDirection;

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
        m_transform.rotation = Quaternion.LookRotation(m_localDirection ? transform.rotation * m_direction : m_direction);
    }
}
