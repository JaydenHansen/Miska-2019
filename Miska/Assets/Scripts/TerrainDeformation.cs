using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDeformation : MonoBehaviour
{
    public Terrain m_terrain;
    public Player m_player;
    public float m_displacement;

    bool[,] m_changed;
    int m_resolution;

    // Start is called before the first frame update
    void Start()
    {
        m_terrain = GetComponent<Terrain>();        
        m_terrain.terrainData = Instantiate(m_terrain.terrainData);
        m_resolution = m_terrain.terrainData.heightmapResolution;

        float[,] heights = m_terrain.terrainData.GetHeights(0, 0, m_resolution, m_resolution);
        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                heights[x, y] += m_displacement / 600f;
            }
        }

        m_terrain.terrainData.SetHeights(0, 0, heights);

        m_changed = new bool[m_resolution, m_resolution];
    }

    // Update is called once per frame
    void Update()
    {
        float[,] heights = m_terrain.terrainData.GetHeights(0,0, m_resolution, m_resolution);
        int playerY = Mathf.RoundToInt(m_player.transform.position.x * ( m_resolution / 500f));
        int playerX = Mathf.RoundToInt(m_player.transform.position.z * ( m_resolution / 500f));

        if (playerX < m_resolution && playerY < m_resolution)
        {
            for (int x = playerX - 1; x < playerX + 1; x++)
            {
                for (int y = playerY - 1; y < playerY + 1; y++)
                {
                    x = Mathf.Clamp(x, 0, m_resolution);
                    y = Mathf.Clamp(y, 0, m_resolution);
                    if (!m_changed[x, y])
                    {
                        Vector2 playerCoords = new Vector2(m_player.transform.position.x, m_player.transform.position.z);
                        Vector2 vertexCoords = new Vector2(playerX * (500f / m_resolution), playerY * (500f / m_resolution));
                        if ((playerCoords - vertexCoords).sqrMagnitude > (m_player.CharacterController.radius * m_player.CharacterController.radius))
                        {
                            float oldHeight = heights[x, y];
                            float newHeight = oldHeight - (m_displacement / 600f);

                            float[,] newHeights = new float[1, 1] { { heights[x, y] - (m_displacement / 600f) } };
                            //heights[x, y] -= m_displacement / 600f;

                            m_terrain.terrainData.SetHeights(y, x, newHeights);
                            //m_terrain.terrainData.

                            m_changed[x, y] = true;
                        }
                    }
                }
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    float[,] heights = m_terrain.terrainData.GetHeights(0, 0, m_terrain.terrainData.heightmapResolution, m_terrain.terrainData.heightmapResolution);
    //    Gizmos.DrawSphere(new Vector3(1025 * (500f / m_terrain.terrainData.heightmapResolution), 0, 1025 * (500f / m_terrain.terrainData.heightmapResolution)), 1);
    //}
}
