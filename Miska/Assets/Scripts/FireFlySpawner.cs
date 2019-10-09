using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        for (int i = 0; i < m_count; i++)
        {
            FireFly ff = Instantiate(m_fireFly, transform.position + (Random.insideUnitSphere * m_radius), Quaternion.identity, transform).GetComponent<FireFly>();
            ff.m_player = m_player;
            ff.m_terrain = m_terrain;
            ff.m_camera = m_camera;
            ff.m_spawner = this;    
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, m_radius);
    }

    public float DistanceFromSpawn(Vector3 pos, out Vector3 direction)
    {
        direction = new Vector3(pos.x, 0, pos.z) - new Vector3(transform.position.x, 0, transform.position.z);
        float distance = direction.magnitude;
        direction = direction / distance;
        return distance;
    }
}
