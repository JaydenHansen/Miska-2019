using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Butterfly : MonoBehaviour
{
    [Header("Points/Curve")]
    public Transform m_start;
    public Transform m_end;

    [Range(2, 10)]
    public int m_pointCount = 10;
    public Vector3 m_extents;

    [Header("Butterfly")]
    public Transform m_butterfly;
    public float m_speed;

    public bool m_regenerate;

    Vector3[] m_points;
    float m_timer;
    // Start is called before the first frame update
    void Start()
    {
        GeneratePoints();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_regenerate)
        {
            GeneratePoints();
            m_regenerate = false;
        }

        if (m_butterfly)
        {
            m_timer += Time.deltaTime;
            m_butterfly.transform.position = getCasteljauPoint(m_pointCount - 1, 0, Mathf.Clamp01(m_timer / m_speed));
            if (m_timer >= m_speed)
            {
                m_timer = 0;
            }
        }
    }

    void GeneratePoints()
    {
        m_points = new Vector3[m_pointCount];
        m_points[0] = m_start.position;
        for (int i = 1; i < m_pointCount - 1; i++)
        {
            m_points[i] = Vector3.Lerp(m_start.position, m_end.position, Random.Range(0f, 1f)) + new Vector3(m_extents.x * Random.Range(-1, 1), m_extents.y * Random.Range(-1, 1), m_extents.z * Random.Range(-1, 1));
        }
        m_points[m_pointCount - 1] = m_end.position;
    }

    private Vector3 getCasteljauPoint(int r, int i, float t)
    {
        if (r == 0)
        {           
            return m_points[i];
        }

        Vector3 p1 = getCasteljauPoint(r - 1, i, t);
        Vector3 p2 = getCasteljauPoint(r - 1, i + 1, t);

        return new Vector3((1f - t) * p1.x + t * p2.x, (1f - t) * p1.y + t * p2.y, (1f - t) * p1.z + t * p2.z);
    }

    private void OnDrawGizmos()
    {
        if (m_points.Length == 0 || m_points.Length != m_pointCount)
            GeneratePoints();

        Gizmos.color = Color.red;

        int steps = 100;
        for (int i = 0; i < steps; i++)
        {
            Vector3 point = getCasteljauPoint(m_pointCount - 1, 0, (float)i / (float)steps);
            Gizmos.DrawWireSphere(point, 0.1f);
        }

        Gizmos.color = Color.green;

        for (int i = 0; i < m_pointCount; i++)
        {
            Gizmos.DrawWireSphere(m_points[i], 0.1f);
        }
    }
}
