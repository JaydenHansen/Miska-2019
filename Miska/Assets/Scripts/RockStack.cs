using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Knocks over the attaches pickup rocks in a direction
/// </summary>
public class RockStack : MonoBehaviour
{
    public float m_knockOverStrength;
    [Tooltip("Direction relative to the gameobject")]
    public Vector3 m_knockOverDirection;
    public Pickup[] m_rocks;

    private void Start()
    {
        m_knockOverDirection.Normalize();
        m_knockOverDirection = transform.rotation * m_knockOverDirection;
    }

    public void KnockOver()
    {
        foreach(Pickup rock in m_rocks) // applies force on each rock in the knockover direction
        {
            rock.Rigidbody.isKinematic = false;
            rock.Rigidbody.AddForce(m_knockOverStrength * m_knockOverDirection);
        }
    }
}
