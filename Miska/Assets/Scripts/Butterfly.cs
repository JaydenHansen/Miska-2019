using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves objects randomly between a set of waypoints using bezier curves
/// </summary>

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
    Vector3[] m_velocity;
    Vector3[] m_nextPoint;
    int[] m_currentStep;

    // Start is called before the first frame update
    void Start()
    {
        // array setup
        m_start = new Transform[m_butterflies.Length];
        m_end = new Transform[m_butterflies.Length];
        m_points = new Vector3[m_butterflies.Length][];
        m_lastPos = new Vector3[m_butterflies.Length];
        m_velocity = new Vector3[m_butterflies.Length];
        m_nextPoint = new Vector3[m_butterflies.Length];
        m_currentStep = new int[m_butterflies.Length];

        // starting targets for each butterfly
        for (int i = 0; i < m_butterflies.Length; i++)
        {
            List<Transform> tempList = new List<Transform>(m_waypoints);

            // gets random start and end point out of the avaliable waypoints
            m_start[i] = tempList[Random.Range(0, tempList.Count)];
            tempList.Remove(m_start[i]);
            m_end[i] = tempList[Random.Range(0, tempList.Count)];

            GeneratePoints(i); // generate points for the current butterfly
        }

        // if using velocity butterflies will seek between steps along the curve
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
        for (int i = 0; i < m_butterflies.Length; i++) // for each butterfly
        {
            m_velocity[i] += (m_nextPoint[i] - m_butterflies[i].position).normalized * m_baseSpeed * Time.deltaTime; // seek to the next point
            m_butterflies[i].position += m_velocity[i] * Time.deltaTime; // move the current butterfly
            m_velocity[i] -= m_velocity[i] * m_friction * Time.deltaTime; // apply friction

            m_butterflies[i].rotation = Quaternion.Lerp(m_butterflies[i].rotation, Quaternion.LookRotation(-m_velocity[i]), Time.deltaTime * m_rotationSpeed); // lerps between the current rotation and the look rotation of the velocity

            // if the butterfly has reached the next point
            if ((m_butterflies[i].position - m_nextPoint[i]).magnitude < 0.05)
            {
                m_nextPoint[i] = getCasteljauPoint(m_pointCount - 1, 0, m_currentStep[i]++ / (float)m_stepCount,i); // get the next step of the curve

                // if the butterfly has reached the end of the curve
                if (m_currentStep[i] == m_stepCount)
                {
                    FindNextPoint(i); // get a new target
                    m_currentStep[i] = 0; 
                    m_nextPoint[i] = getCasteljauPoint(m_pointCount - 1, 0, m_currentStep[i]++ / (float)m_stepCount,i);
                }
            }            
        }
    }

    /// <summary>
    /// Generate a random set of points for the bezier curve calculation
    /// </summary>
    /// <param name="index">The index of the current butterfly</param>
    void GeneratePoints(int index)
    {
        m_points[index] = new Vector3[m_pointCount];
        m_points[index][0] = m_start[index].position; // the starting point will be the location of the starting waypoint

        // get a random set of points along the line between the start and end waypoints
        for (int i = 1; i < m_pointCount - 1; i++)
        {
            m_points[index][i] = Vector3.Lerp(m_start[index].position, m_end[index].position, Random.Range(0f, 1f)) + new Vector3(m_extents.x * Random.Range(-1, 1), m_extents.y * Random.Range(-1, 1), m_extents.z * Random.Range(-1, 1)) + m_offset;
        }

        m_points[index][m_pointCount - 1] = m_end[index].position; // the end point will be the location of the end waypoint
    }

    /// <summary>
    /// Make the old end point the new starting point and get a new end point
    /// </summary>
    /// <param name="index">The index of the current butterfly</param>
    void FindNextPoint(int index)
    {
        List<Transform> tempList = new List<Transform>(m_waypoints);
        m_start[index] = m_end[index]; // start the path from the previous end point
        tempList.Remove(m_start[index]); // can't path to the same point

        m_end[index] = tempList[Random.Range(0, tempList.Count)];

        GeneratePoints(index); // generate a new set of random points
    }

    /// <summary>
    /// Uses De Casteljau's algorithm to calcualte the position on a bezier curve at a specific time
    /// </summary>
    /// <param name="r">The number of points in that make up the bezier curve</param>
    /// <param name="i">The starting point index</param>
    /// <param name="t">The point along the path to calculate (0-1)</param>
    /// <param name="index">The index of the current butterfly</param>
    /// <returns></returns>
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

    /// <summary>
    /// Debug drawing of the bezier curve
    /// </summary>
    private void OnDrawGizmos()
    {
        if (m_showDebug && m_points != null)
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