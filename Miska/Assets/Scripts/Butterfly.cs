using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Butterfly : MonoBehaviour
{
    [Header("Points/Curve")]
    public Transform[] m_waypoints;

    [Range(2, 10)]
    public int m_pointCount = 10;
    public Vector3 m_extents;
    public Vector3 m_offset;

    [Header("Butterfly")]
    public Transform[] m_butterflies;
    public float m_baseSpeed;
    public float m_rotationSpeed;
    public bool m_useVelocity;
    public float m_friction;
    public int m_stepCount;

    [Header("Debug")]
    //public bool m_regenerate;
    public bool m_showDebug;

    Transform[] m_start;
    Transform[] m_end;
    Vector3[][] m_points;
    float m_timer;
    Vector3[] m_lastPos;
    float m_speed;
    Vector3[] m_velocity;
    Vector3[] m_nextPoint;
    int[] m_currentStep;

    // Start is called before the first frame update
    void Start()
    {
        m_start = new Transform[m_butterflies.Length];
        m_end = new Transform[m_butterflies.Length];
        m_points = new Vector3[m_butterflies.Length][];
        m_lastPos = new Vector3[m_butterflies.Length];
        m_velocity = new Vector3[m_butterflies.Length];
        m_nextPoint = new Vector3[m_butterflies.Length];
        m_currentStep = new int[m_butterflies.Length];

        for (int i = 0; i < m_butterflies.Length; i++)
        {
            List<Transform> tempList = new List<Transform>(m_waypoints);
            m_start[i] = tempList[Random.Range(0, tempList.Count)];
            tempList.Remove(m_start[i]);
            m_end[i] = tempList[Random.Range(0, tempList.Count)];
            GeneratePoints(i);
        }

        if (m_useVelocity)
        {
            for (int i = 0; i < m_butterflies.Length; i++)
            {
                m_currentStep[i] = 0;
                m_nextPoint[i] = getCasteljauPoint(m_pointCount - 1, 0, m_currentStep[i]++ / m_stepCount, i);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_butterflies.Length; i++)
        {
            m_velocity[i] += (m_nextPoint[i] - m_butterflies[i].position).normalized * m_baseSpeed * Time.deltaTime;
            m_butterflies[i].position += m_velocity[i] * Time.deltaTime;
            m_velocity[i] -= m_velocity[i] * m_friction * Time.deltaTime;

            m_butterflies[i].rotation = Quaternion.Lerp(m_butterflies[i].rotation, Quaternion.LookRotation(-m_velocity[i]), Time.deltaTime * m_rotationSpeed);

            if ((m_butterflies[i].position - m_nextPoint[i]).magnitude < 0.05)
            {
                m_nextPoint[i] = getCasteljauPoint(m_pointCount - 1, 0, m_currentStep[i]++ / (float)m_stepCount,i);
                if (m_currentStep[i] == m_stepCount)
                {
                    FindNextPoint(i);
                    m_currentStep[i] = 0;
                    m_nextPoint[i] = getCasteljauPoint(m_pointCount - 1, 0, m_currentStep[i]++ / (float)m_stepCount,i);
                }
            }            
        }
    }

    void GeneratePoints(int index)
    {
        m_points[index] = new Vector3[m_pointCount];
        m_points[index][0] = m_start[index].position;
        for (int i = 1; i < m_pointCount - 1; i++)
        {
            m_points[index][i] = Vector3.Lerp(m_start[index].position, m_end[index].position, Random.Range(0f, 1f)) + new Vector3(m_extents.x * Random.Range(-1, 1), m_extents.y * Random.Range(-1, 1), m_extents.z * Random.Range(-1, 1)) + m_offset;
        }
        m_points[index][m_pointCount - 1] = m_end[index].position;
    }

    void FindNextPoint(int index)
    {
        List<Transform> tempList = new List<Transform>(m_waypoints);
        m_start[index] = m_end[index];
        tempList.Remove(m_start[index]);
        m_end[index] = tempList[Random.Range(0, tempList.Count)];

        m_speed = (m_start[index].position - m_end[index].position).magnitude * m_baseSpeed;
        GeneratePoints(index);
    }

    private Vector3 getCasteljauPoint(int r, int i, float t, int index)
    {
        if (r == 0)
        {
            return m_points[index][i];
        }

        Vector3 p1 = getCasteljauPoint(r - 1, i, t, index);
        Vector3 p2 = getCasteljauPoint(r - 1, i + 1, t, index);

        return new Vector3((1f - t) * p1.x + t * p2.x, (1f - t) * p1.y + t * p2.y, (1f - t) * p1.z + t * p2.z);
    }

    private void OnDrawGizmos()
    {
        if (m_showDebug)
        {
            for (int j = 0; j < m_butterflies.Length; j++)
            {
                if (m_points[j].Length == 0 || m_points[j].Length != m_pointCount)
                    GeneratePoints(j);

                Gizmos.color = Color.red;

                int steps = 50;
                for (int i = 0; i < steps; i++)
                {
                    Vector3 point = getCasteljauPoint(m_pointCount - 1, 0, (float)i / (float)steps, j);
                    Gizmos.DrawWireSphere(point, 0.1f);
                }

                Gizmos.color = Color.green;

                for (int i = 0; i < m_pointCount; i++)
                {
                    Gizmos.DrawWireSphere(m_points[j][i], 0.1f);
                }
            }
        }
    }
}