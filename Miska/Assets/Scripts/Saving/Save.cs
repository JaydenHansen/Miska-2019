using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// a series of variables to be saves
/// </summary>
[System.Serializable]
public class Save
{
    public SerializableVector3 m_playerPosition;
    public SerializableVector3 m_cameraRotation;

    public List<int> m_trashCanTrashLeft = new List<int>();
    public List<List<bool>> m_trashActive = new List<List<bool>>();

}

/// <summary>
/// an array of which collectables have been picked up
/// </summary>
[System.Serializable]
public class CollectableSave
{
    public bool[] m_collectables;
}

/// <summary>
/// A vector 3 that is serializable
/// </summary>
[System.Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    /// <summary>
    /// creates a serializable vector from a regular unity vector
    /// </summary>
    /// <param name="old">the unity vector</param>
    public void GetFromVector3(Vector3 old)
    {
        x = old.x;
        y = old.y;
        z = old.z;
    }

    /// <summary>
    /// creates a unity vector from a serializable vector
    /// </summary>
    /// <returns>the unity vector</returns>
    public Vector3 GetVector3()
    {
        return new Vector3(x, y, z);
    }
}