using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The area that the dog stays in
/// </summary>
public class DogArea : MonoBehaviour
{
    public Bounds m_bounds;

    // debug draw the bounds
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(m_bounds.center, m_bounds.size);
    }

    /// <summary>
    /// Checks if the bounds contain a position
    /// </summary>
    /// <param name="position">The position to be checked</param>
    /// <returns>if the position is in the bounds</returns>
    public bool Contains(Vector3 position)
    {
        return m_bounds.Contains(position);
    }
}
