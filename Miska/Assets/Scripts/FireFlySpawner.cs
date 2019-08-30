using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlySpawner : MonoBehaviour
{
    public GameObject m_fireFly;
    public Transform m_player;
    public Transform m_camera;
    public Terrain m_terrain;
    public Transform m_spawnPosition;
    public int m_count;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_count; i++)
        {
            FireFly ff = Instantiate(m_fireFly, m_spawnPosition).GetComponent<FireFly>();
            ff.m_player = m_player;
            ff.m_terrain = m_terrain;
            ff.m_camera = m_camera;
        }
    }
}
