#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor script to combine meshes into a single object (or multiple if there are too many verts)
/// Warning kinda janky
/// </summary>
public class CombineMeshes : MonoBehaviour
{
    public MeshFilter[] m_subMeshes;
    public bool m_useChildren;
    public bool m_destroySub;
    public bool m_destroyScript;
    public bool m_saveMesh;

    MeshFilter m_combined;

    /// <summary>
    /// The function to start combining the meshes
    /// </summary>
    public void Combine()
    {
        if (m_useChildren) // if the children of the gameobject should be used as the submeshes
        {
            m_subMeshes = GetComponentsInChildren<MeshFilter>();
        }

        Dictionary<Mesh, List<MeshFilter>> meshTypes = new Dictionary<Mesh, List<MeshFilter>>(); // splits the submeshes based on their mesh, grouping those with the same mesh
        for (int i = 0; i < m_subMeshes.Length; i++)
        {
            if (m_subMeshes[i].gameObject != gameObject) // checks that it doesn't include the parent object
            {
                if (meshTypes.ContainsKey(m_subMeshes[i].sharedMesh)) // if the mesh of the current object is already in the dictionary
                    meshTypes[m_subMeshes[i].sharedMesh].Add(m_subMeshes[i]);
                else
                    meshTypes.Add(m_subMeshes[i].sharedMesh, new List<MeshFilter> { m_subMeshes[i] }); // create a new entry using the mesh of the object and it's mesh filter
            }
        }

        // for each of the mesh types
        foreach (var entry in meshTypes)
        {
            List<MeshFilter> sameMeshes = entry.Value; // list of objects that share a mesh

            int totalVertexCount = sameMeshes[0].sharedMesh.vertexCount * sameMeshes.Count; // get's the total vertex count of all the meshes combined
            if (totalVertexCount > 65000 || true) // unity limits the amount of vertices in a mesh to ~65000 (TEMP OVERRIDE)
            {
                int meshesPerObject = Mathf.FloorToInt(65000f / sameMeshes[0].sharedMesh.vertexCount); // calculates the amount of submeshes that can be combined into a single object
                int objectCount = Mathf.CeilToInt(totalVertexCount / 65000f); // calculates the amount of objects needed for all submeshes to be combined
          
                //CombineInstance[,] combineStorage = new CombineInstance[objectCount, meshesPerObject];
                List<List<CombineInstance>> combineStorage = new List<List<CombineInstance>>();
                for (int i = 0; i < objectCount; i++)
                    combineStorage.Add(new List<CombineInstance>());

                for (int i = 0; i < sameMeshes.Count; i++)
                {
                    if (sameMeshes[i].sharedMesh.subMeshCount == 1) // if the current mesh only has a single sub mesh
                    {
                        int index = Mathf.FloorToInt(i / (float)meshesPerObject); // the index of the combined objects that the current mesh will fit into
                        CombineInstance newInstance = new CombineInstance();
                        newInstance.subMeshIndex = 0;
                        newInstance.mesh = sameMeshes[i].sharedMesh;
                        newInstance.transform = sameMeshes[i].transform.localToWorldMatrix;
                        combineStorage[index].Add(newInstance);
                        sameMeshes[i].gameObject.SetActive(false);
                    }
                    else // might not work correctly
                    {
                        int index = Mathf.FloorToInt(i / (float)meshesPerObject);
                        for (int j = 0; j < sameMeshes[i].sharedMesh.subMeshCount; j++)
                        {
                            CombineInstance newInstance = new CombineInstance();
                            newInstance.subMeshIndex = j;
                            newInstance.mesh = sameMeshes[i].sharedMesh;
                            newInstance.transform = sameMeshes[i].transform.localToWorldMatrix;
                            combineStorage[index].Add(newInstance);
                        }
                        sameMeshes[i].gameObject.SetActive(false);
                    }
                }

                combineStorage.TrimExcess();

                for (int i = 0; i < objectCount; i++) // for the total amount of combined meshes
                {
                    CombineInstance[] temp = combineStorage[i].ToArray(); // get list of combine instances for the current combined mesh
                   
                    Mesh combinedMesh = new Mesh();

                    if (sameMeshes[0].sharedMesh.subMeshCount == 1) // if there is only a single submesh for the meshes in the list
                    {
                        combinedMesh.CombineMeshes(temp);
                    }
                    else
                    { 
                        combinedMesh.CombineMeshes(temp, false);

                        List<int[]> subMeshes = new List<int[]>();
                        for (int j = 0; j < combinedMesh.subMeshCount; j++)
                        {
                            subMeshes.Add(combinedMesh.GetTriangles(j));
                        }

                        combinedMesh.subMeshCount = sameMeshes[0].sharedMesh.subMeshCount;

                        List<List<int>> newSubMeshes = new List<List<int>>();
                        for (int j = 0; j < combinedMesh.subMeshCount; j++)
                        {
                            List<int> currentSubMesh = new List<int>();
                            int index = 0;
                            for (int k = 0; k < subMeshes.Count; k++)
                            {
                                if (index == j)
                                {
                                    currentSubMesh.AddRange(subMeshes[k]);
                                }

                                if (index++ == combinedMesh.subMeshCount)
                                    index = 0;
                            }
                            newSubMeshes.Add(currentSubMesh);
                        }

                        for (int j = 0; j < newSubMeshes.Count; j++)
                        {
                            combinedMesh.SetTriangles(newSubMeshes[j], j);
                        }
                    }

                    // finish mesh setup
                    combinedMesh.Optimize();
                    combinedMesh.RecalculateBounds();
                    combinedMesh.RecalculateNormals();

                    combinedMesh.name = sameMeshes[0].sharedMesh.name + " combined " + i.ToString();

                    // create a game object for the new combined mesh
                    GameObject newObject = new GameObject(sameMeshes[0].sharedMesh.name + " combined " + i.ToString()); 
                    newObject.transform.parent = gameObject.transform;

                    MeshFilter newMeshFilter = newObject.AddComponent<MeshFilter>();
                    newMeshFilter.sharedMesh = combinedMesh;

                    newObject.AddComponent<MeshRenderer>().sharedMaterials = sameMeshes[i].GetComponent<Renderer>().sharedMaterials;
                }                
            }
            else
            {
                //m_combined = GetComponent<MeshFilter>();

                //CombineInstance[] combineStorage = new CombineInstance[m_subMeshes.Length];

                //for (int i = 0; i < m_subMeshes.Length; i++)
                //{
                //    combineStorage[i].subMeshIndex = 0;
                //    combineStorage[i].mesh = m_subMeshes[i].sharedMesh;
                //    combineStorage[i].transform = m_subMeshes[i].transform.localToWorldMatrix;
                //    //m_subMeshes[i].gameObject.SetActive(false);
                //}

                //Mesh combinedMesh = new Mesh();
                //combinedMesh.CombineMeshes(combineStorage);
                //combinedMesh.name = "Combined Grass";
                //m_combined.sharedMesh = combinedMesh;
                //gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// combine child meshes into a parent mesh
    /// </summary>
    public void CombineIntoParent()
    {
        m_subMeshes = GetComponentsInChildren<MeshFilter>();        
        if (m_subMeshes.Length == 0)
        {
            Debug.Log("No MeshFilters found");
            return;
        }

        List<CombineInstance> combineStorage = new List<CombineInstance>();
        for (int i = 0; i < m_subMeshes.Length; i++)
        {
            CombineInstance newInstance = new CombineInstance();
            newInstance.mesh = m_subMeshes[i].sharedMesh;

            Matrix4x4 matrix = Matrix4x4.identity;
            if (i != 0) // if the current mesh is a child mesh
                matrix.SetTRS(m_subMeshes[i].transform.localPosition, m_subMeshes[i].transform.localRotation, m_subMeshes[i].transform.localScale); // uses the local values for the matrix

            newInstance.transform = matrix;
            combineStorage.Add(newInstance);
            if (m_destroySub && i != 0) // destroies the old child mesh
            {
                DestroyImmediate(m_subMeshes[i].gameObject);
            }
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineStorage.ToArray());

        if (m_saveMesh) // if the mesh should be saved into assets
        {
            string path = EditorUtility.SaveFilePanel("Save Combined Mesh", "Assets/", "combined", "asset");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);

            AssetDatabase.CreateAsset(combinedMesh, path); // creates an asset for the combined mesh
            AssetDatabase.SaveAssets();                   
        }
        else
        {
            m_subMeshes[0].sharedMesh = combinedMesh;
        }

        if (m_destroyScript) // if the combine script should be auto removed
        {
            DestroyImmediate(this);
        }
    }
}

#endif

#if UNITY_EDITOR
/// <summary>
/// Custom inspector for the combine meshes
/// </summary>
[CustomEditor(typeof(CombineMeshes))]
[CanEditMultipleObjects]
public class CombineMeshesInspector : Editor
{
    SerializedObject m_script;

    private void OnEnable()
    {
        m_script = serializedObject;
    }

    public override void OnInspectorGUI()
    {
        SerializedProperty useChildren = m_script.FindProperty("m_useChildren");
        SerializedProperty destroySub = m_script.FindProperty("m_destroySub");
        SerializedProperty destroyScript = m_script.FindProperty("m_destroyScript");
        SerializedProperty saveMesh = m_script.FindProperty("m_saveMesh");

        //DrawDefaultInspector();
        useChildren.boolValue = EditorGUILayout.Toggle("Use Children", useChildren.boolValue);
        destroySub.boolValue = EditorGUILayout.Toggle("Destroy SubMeshes", destroySub.boolValue);
        destroyScript.boolValue = EditorGUILayout.Toggle("Destroy Script", destroyScript.boolValue);
        saveMesh.boolValue = EditorGUILayout.Toggle("Save Mesh", saveMesh.boolValue);        

        if (!useChildren.boolValue)
        {
            SerializedProperty subMeshes = serializedObject.FindProperty("m_subMeshes");
            EditorGUILayout.PropertyField(subMeshes, true);
        }
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Combine"))
        {
            foreach (Object script in m_script.targetObjects)
            {
                ((CombineMeshes)script).Combine();
            }
        }

        if (GUILayout.Button("Combine into parent"))
        {
            foreach (Object script in m_script.targetObjects)
            {
                ((CombineMeshes)script).CombineIntoParent();
            }
        }
    }
}
#endif