using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockStack : MonoBehaviour
{
    public float m_knockOverStrength;
    [Tooltip("Direction relative to the gameobject")]
    public Vector3 m_knockOverDirection;
    public Pickup[] m_rocks;

    private void Start()
    {
        m_knockOverDirection = transform.rotation * m_knockOverDirection;
    }

    public void KnockOver()
    {
        foreach(Pickup rock in m_rocks)
        {
            rock.Rigidbody.isKinematic = false;
            rock.Rigidbody.AddForce(m_knockOverStrength * m_knockOverDirection);
        }
    }
}
