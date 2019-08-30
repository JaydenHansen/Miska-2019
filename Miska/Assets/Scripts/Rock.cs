using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Pickup
{
    public RockStacker m_stacker;
    public float m_delay;
    public bool m_freezeOnTouch;

    bool m_stacked;
    private float m_timer;

    public bool Stacked
    {
        get { return m_stacked; }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_rigidbody = GetComponent<Rigidbody>();    
    }

    private void OnCollisionStay(Collision collision)
    {
        if (m_freezeOnTouch)
        {
            if (collision.gameObject.tag == "Ground" && !m_stacker.BasePlaced && m_target == null)
            {
                m_rigidbody.isKinematic = true;
                m_stacker.BasePlaced = true;
                m_stacked = true;
            }
            else
            {
                Rock otherRock = collision.gameObject.GetComponent<Rock>();
                if (otherRock && otherRock.Stacked && m_target == null)
                {
                    m_timer += Time.deltaTime;

                    if (m_timer >= m_delay)
                    {
                        m_rigidbody.isKinematic = true;
                        m_stacked = true;
                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        m_timer = 0;
    }    
}
