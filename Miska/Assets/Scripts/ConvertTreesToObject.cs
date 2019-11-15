using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Converts the trees of the attached terrain to gameobjects
/// </summary>
[RequireComponent(typeof(Terrain))]
public class ConvertTreesToObject : MonoBehaviour
{
    public Transform m_parent;

    public void Convert()
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData data = terrain.terrainData;        
        
        foreach(TreeInstance tree in data.treeInstances) // for each tree in the terrain data
        {
            Vector3 position = new Vector3(tree.position.x * data.size.x, tree.position.y * data.size.y, tree.position.z * data.size.z); // the position of the gameobject is the position of the tree scaled by the size of the terrain

            if (m_parent) // if the trees should be placed in a parent object
                Instantiate(data.treePrototypes[tree.prototypeIndex].prefab, position, Quaternion.identity, m_parent); // gets the prefab of the current tree from the terrain data and spawns it
            else
            Instantiate(data.treePrototypes[tree.prototypeIndex].prefab, position, Quaternion.identity);
        }
    }
}
