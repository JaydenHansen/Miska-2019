using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Butterfly : MonoBehaviour
{
    [Header("Points/Curve")]
    public Transform[] m_waypoints;
    Transform m_start;
    Transform m_end;

    [Range(2, 10)]
    public int m_pointCount = 10;
    public Vector3 m_extents;
    public Vector3 m_offset;

    [Header("Butterfly")]
    public Transform m_butterfly;
    public float m_baseSpeed;
    public float m_rotationSpeed;
    public bool m_useVelocity;
    public float m_friction;
    public int m_stepCount;

    [Header("Debug")]
    public bool m_regenerate;
    public bool m_showDebug;

    Vector3[] m_points;
    float m_timer;
    Vector3 m_lastPos;
    float m_speed;
    Vector3 m_velocity;
    Vector3 m_nextPoint;
    int m_currentStep;

    // Start is called before the first frame update
    void Start()
    {
        List<Transform> tempList = new List<Transform>(m_waypoints);
        m_start = tempList[Random.Range(0, tempList.Count)];
        tempList.Remove(m_start);
        m_end = tempList[Random.Range(0, tempList.Count)];
        GeneratePoints();

        if (m_useVelocity)
        {
            m_currentStep = 0;
            m_nextPoint = getCasteljauPoint(m_pointCount - 1, 0, m_currentStep++ / m_stepCount);
        }
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
            if (m_useVelocity)
            {
                m_velocity += (m_nextPoint - m_butterfly.position).normalized * m_baseSpeed * Time.deltaTime;
                m_butterfly.position += m_velocity * Time.deltaTime;
                m_velocity -= m_velocity * m_friction * Time.deltaTime;

                m_butterfly.rotation = Quaternion.Lerp(m_butterfly.rotation, Quaternion.LookRotation(-m_velocity), Time.deltaTime * m_rotationSpeed);

                if ((m_butterfly.position - m_nextPoint).magnitude < 0.05)
                {
                    m_nextPoint = getCasteljauPoint(m_pointCount - 1, 0, m_currentStep++ / (float)m_stepCount);
                    if (m_currentStep == m_stepCount)
                    {
                        FindNextPoint();
                        m_currentStep = 0;
                        m_nextPoint = getCasteljauPoint(m_pointCount - 1, 0, m_currentStep++ / (float)m_stepCount);
                    }
                }
            }
            else
            {
                m_timer += Time.deltaTime;
                m_butterfly.position = getCasteljauPoint(m_pointCount - 1, 0, Mathf.Clamp01(m_timer / m_speed));

                m_butterfly.rotation = Quaternion.Lerp(m_butterfly.rotation, Quaternion.LookRotation(-(m_lastPos - m_butterfly.position).normalized), Time.deltaTime * m_rotationSpeed);

                m_lastPos = m_butterfly.position;
                if (m_timer >= m_speed)
                {
                    m_timer = 0;
                    if (m_waypoints.Length > 0)
                    {
                        FindNextPoint();
                    }
                }
            }
        }
    }

    void GeneratePoints()
    {
        m_points = new Vector3[m_pointCount];
        m_points[0] = m_start.position;
        for (int i = 1; i < m_pointCount - 1; i++)
        {
            m_points[i] = Vector3.Lerp(m_start.position, m_end.position, Random.Range(0f, 1f)) + new Vector3(m_extents.x * Random.Range(-1, 1), m_extents.y * Random.Range(-1, 1), m_extents.z * Random.Range(-1, 1)) + m_offset;
        }
        m_points[m_pointCount - 1] = m_end.position;
    }

    void FindNextPoint()
    {
        List<Transform> tempList = new List<Transform>(m_waypoints);
        m_start = m_end;
        tempList.Remove(m_start);
        m_end = tempList[Random.Range(0, tempList.Count)];

        m_speed = (m_start.position - m_end.position).magnitude * m_baseSpeed;
        GeneratePoints();
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
        if (m_showDebug)
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
}