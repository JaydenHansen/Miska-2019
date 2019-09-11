using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogArea : MonoBehaviour
{
    public Bounds m_bounds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + m_bounds.center, m_bounds.size);
    }

    public bool Contains(Vector3 position)
    {
        return m_bounds.Contains(position);
    }
}
