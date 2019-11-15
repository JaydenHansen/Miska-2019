using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns firefly objects in an area
/// </summary>
public class FireFlySpawner : MonoBehaviour
{
    public GameObject m_fireFly;
    public Transform m_player;
    public Transform m_camera;
    public Terrain m_terrain;
    public int m_count;
    public float m_radius;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_count; i++) // spawns a specific amount of fireflies
        {
            FireFly ff = Instantiate(m_fireFly, transform.position + (Random.insideUnitSphere * m_radius), Quaternion.identity, transform).GetComponent<FireFly>(); // spawns firefly randomly in area
            // firefly setup
            ff.m_player = m_player;
            ff.m_terrain = m_terrain;
            ff.m_camera = m_camera;
            ff.m_spawner = this;    
        }
    }

    /// <summary>
    /// Debug draw the area
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, m_radius);
    }

    /// <summary>
    /// Gets the current distance from the spawners
    /// </summary>
    /// <param name="pos">the position of the firefly</param>
    /// <param name="direction">The direction back towards the centre</param>
    /// <returns></returns>
    public float DistanceFromSpawn(Vector3 pos, out Vector3 direction)
    {
        direction = transform.position - pos;
        float distance = direction.magnitude;
        direction = direction / distance;
        return distance;
    }
}
