using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockStacker : MonoBehaviour
{
    public Pickup[] m_rocks;
    public float m_boundsOffset;
    public int m_maxTouchingGround = 1;
    public float m_stackDelay;
    public float m_knockOverStrength;
    public Vector3 m_knockOverDirection;
    public VoidEvent m_onStacked;

    bool m_hasBeenStacked;
    float m_stackTimer;
    bool m_basePlaced;

   // public AK.Wwise.Event m_rockfallSound;

    public bool BasePlaced
    {
        get { return m_basePlaced; }
        set { m_basePlaced = value; }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_hasBeenStacked)
        {
            bool notInHand = true;
            foreach (Pickup rockPickup in m_rocks)
            {
                if (rockPickup.Target != null)
                {
                    notInHand = false;
                    break;
                }
            }
            if (notInHand)
            {
                int touchingOther = 0;
                bool allTouching = true;

                foreach (Pickup rock in m_rocks)
                {
                    Collider[] collisions = Physics.OverlapBox(rock.transform.position, rock.Collider.bounds.extents + new Vector3(m_boundsOffset, m_boundsOffset, m_boundsOffset), transform.rotation);
                    if (collisions.Length > 1)
                    {
                        foreach (Collider collision in collisions)
                        {
                            if (collision.tag != "Pickup")
                            {
                                touchingOther++;
                                break;
                            }
                        }
                    }
                    else
                    {
                        allTouching = false;
                    }
                }

                if (allTouching && touchingOther <= m_maxTouchingGround)
                {
                    Debug.Log("Stacked " + Mathf.RoundToInt((m_stackTimer / m_stackDelay) * 100).ToString() + "%");
                    m_stackTimer += Time.deltaTime;
                    if (m_stackTimer >= m_stackDelay)
                    {
                        m_hasBeenStacked = true;
                        m_onStacked.Invoke();

                        foreach (Pickup rockPickup in m_rocks)
                        {
                            rockPickup.Rigidbody.isKinematic = true;
                            rockPickup.enabled = false;
                        }
                    }
                }
                else
                {
                    m_stackTimer = 0;
                }
            }
        }
    }

    // Breaks cause pickups' colliders aren't initialized
    //private void OnDrawGizmos()
    //{
    //    foreach (Pickup rock in m_rocks)
    //    {
    //        Gizmos.DrawWireCube(rock.transform.position, rock.Collider.bounds.extents * 2 + new Vector3(m_boundsOffset, m_boundsOffset, m_boundsOffset));
    //    }
    //}

    public void KnockOver()
    {        
        foreach(Pickup rockPickup in m_rocks)
        {
            rockPickup.Rigidbody.isKinematic = false;
            rockPickup.Rigidbody.AddForce(m_knockOverDirection * m_knockOverStrength);
        }
        this.enabled = true;
       // m_rockfallSound.Post(gameObject);
    }
}
