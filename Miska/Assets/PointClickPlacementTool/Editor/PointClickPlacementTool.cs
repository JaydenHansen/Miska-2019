using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class PointClickPlacementTool : EditorWindow
{
	public static PointClickPlacementTool window;
	static SceneView.OnSceneFunc onSceneGUIFunc;

    private bool randomRotateYAxis = true;
    private bool randomiseScale = true;
    private float scaleMin = 1f;
    private float scaleMax = 1f;

    private bool enableHelp = false;
	private bool enabled = false;
	private bool useNormalRotation = true;
    private bool showPreview = true;
	private bool customOffset = false;
	private Vector3 buildPos;
	private Vector3 offSet;
	private bool instantiatePrefab = false;
	private Vector2 scrollPos;
	private string newGroupName;
	private GameObject selectedGroup;
	private GameObject currentGameObject;
	private GameObject newSelectedGameObject;
	private int indexname = 0;

    // ADDON
    private bool m_useGroup;
    private float m_angle;
    private int m_placeAmount;

	[MenuItem("Tools/Point Click Placement Tool")]

	public static void ShowWindow()
	{
		window = EditorWindow.GetWindow<PointClickPlacementTool>(false, "PointClickPlacementTool");
	}

	void OnEnable()
	{
		onSceneGUIFunc = this.OnSceneGUI;
		SceneView.onSceneGUIDelegate += onSceneGUIFunc;
	}

	void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= onSceneGUIFunc;
	}

	public void OnSceneGUI(SceneView sceneView)
	{
		if(enabled)
		{
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (!m_useGroup)
            {
                if (Selection.activeObject != null)
                {
                    if (Selection.activeGameObject != null && Selection.activeTransform == null)
                    {
                        newSelectedGameObject = Selection.activeGameObject;
                    }

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit))
                        {
                            buildPos = hit.point + offSet;

                            if (Selection.activeGameObject != null && Selection.activeTransform == null)
                            {
                                instantiatePrefab = true;
                            }
                            if (Selection.activeTransform != null)
                            {
                                instantiatePrefab = false;
                            }

                            if (instantiatePrefab == true)
                            {
                                AddSingle(buildPos, hit);
                            }

                            if (instantiatePrefab == false)
                            {
                                WarnUser();
                            }
                        }
                    }
                }
            }
            else
            {
                List<GameObject> newObjects = new List<GameObject>();
                if (Selection.transforms.Length == 0)
                {
                    for (int i = 0; i < Selection.objects.Length && i < Selection.gameObjects.Length; i++)
                    {
                        if (Selection.objects[i] != null)
                        {
                            if (Selection.gameObjects[i] != null)
                            {
                                newObjects.Add(Selection.gameObjects[i]);
                            }
                        }
                    }
                }

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    for (int i = 0; i < m_placeAmount; i++)
                    { 
                        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition + new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) * m_angle);

                        Transform camera = sceneView.camera.transform;
                        float angle = m_angle * Mathf.Deg2Rad;
                        ray.direction = ray.direction + camera.right * Random.Range(-angle, angle) + camera.up * Random.Range(-angle, angle);


                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            buildPos = hit.point + offSet;                            
                            AddSingle(buildPos, hit, newObjects[Random.Range(0, newObjects.Count)]);                           
                        }
                    }
                }                                  
            }
		}
	}

	private void AddSingle(Vector3 buildPos, RaycastHit clickedObject)
	{
		GameObject prefab = Selection.activeGameObject;
		prefab = PrefabUtility.InstantiatePrefab(prefab.gameObject) as GameObject;

		Vector3 newPos = new Vector3(buildPos.x, buildPos.y, buildPos.z);

		prefab.transform.position = newPos;

		if(useNormalRotation)
		{
			prefab.transform.rotation = Quaternion.FromToRotation(Vector3.up, clickedObject.normal)* prefab.transform.rotation;
		}

        if (randomRotateYAxis)
        {
            prefab.transform.rotation = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), prefab.transform.up) * prefab.transform.rotation;
        }

        if (randomiseScale)
        {
            float myScale = Random.Range(scaleMin, scaleMax);
            prefab.transform.localScale = new Vector3(myScale, myScale, myScale);
        }

        if (selectedGroup != null)
		{
			prefab.transform.parent = selectedGroup.transform;		
		}

		indexname++;

		prefab.name = string.Format("{0}_{1}",prefab.name, indexname);

		Undo.RegisterCreatedObjectUndo(prefab, "Added " + prefab.name + " to Scene");
	}

    private void AddSingle(Vector3 buildPos, RaycastHit clickedObject, GameObject selectedObject)
    {
        GameObject prefab = PrefabUtility.InstantiatePrefab(selectedObject) as GameObject;

        Vector3 newPos = new Vector3(buildPos.x, buildPos.y, buildPos.z);

        prefab.transform.position = newPos;

        if (useNormalRotation)
        {
            prefab.transform.rotation = Quaternion.FromToRotation(Vector3.up, clickedObject.normal) * prefab.transform.rotation;
        }

        if (randomRotateYAxis)
        {
            prefab.transform.rotation = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), prefab.transform.up) * prefab.transform.rotation;
        }

        if (randomiseScale)
        {
            float myScale = Random.Range(scaleMin, scaleMax);
            prefab.transform.localScale = new Vector3(myScale, myScale, myScale);
        }

        if (selectedGroup != null)
        {
            prefab.transform.parent = selectedGroup.transform;
        }

        indexname++;

        prefab.name = string.Format("{0}_{1}", prefab.name, indexname);

        Undo.RegisterCreatedObjectUndo(prefab, "Added " + prefab.name + " to Scene");
    }


    private void WarnUser()
	{
		Debug.Log("You have selected an object in the scene! Please use prefabs or gameobjects from your project window or disable the Point Click Placement Tool");
	}
	

	void OnGUI()
	{
		EditorGUILayout.BeginVertical();
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

        // ENABLE THE TOOL
		if(enableHelp == false)
		{
			EditorGUILayout.HelpBox("Click the Enable Help to get detailed information on each control.", MessageType.Info);
		}

        // ENABLE HELP
		if(enableHelp == false)
		{
			enableHelp = EditorGUILayout.Toggle("Enable Help", enableHelp);
		}

        // DISABLE HELP
		else if(enableHelp == true)
		{
			enableHelp = EditorGUILayout.Toggle("Disable Help", enableHelp);
		}

        // IF TOOL IS ENABLED
		if(enabled == false)
		{
			if(enableHelp)
			{
				EditorGUILayout.HelpBox("Click the Enable button below to Enable the Point Click Placement Tools.", MessageType.Info);
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Make sure there is at least 1 existing collider in your scene.", MessageType.Info);
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Select a prefab or gameobject in the project then left click on the existing collider in the scene to add the selected prefab or gameobject.", MessageType.Info);
			}
			if(GUILayout.Button("Enable"))
			{
				enabled = true;
			}
		}
		else if(enabled == true)
		{
			if(enableHelp)
			{
				EditorGUILayout.HelpBox("Make sure there is at least 1 existing collider in your scene.  Select the Prefab in your Project View, and start clicking in the Scene view to place that object.", MessageType.Info);
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Click the Rotate Object With Surface to enable object rotation with surface.", MessageType.Info);
			}
			useNormalRotation = EditorGUILayout.Toggle("Rotate Object With Surface", useNormalRotation);
            randomRotateYAxis = EditorGUILayout.Toggle("Random rotation on Y axis", randomRotateYAxis);

            EditorGUILayout.Space();
			EditorGUILayout.Space();

			if(enableHelp)
			{
				EditorGUILayout.HelpBox("Click the Disable button below to Disable the Point Click Placement Tools.", MessageType.Info);
			}

			if(GUILayout.Button("Disable Tool"))
			{
				enabled = false;
			}

            if (!m_useGroup)
            {
                if (GUILayout.Button("Use Multi"))
                {
                    m_useGroup = true;
                }
            }
            else
            {
                if (GUILayout.Button("Disable Multi"))
                {
                    m_useGroup = false;
                }
                m_angle = EditorGUILayout.FloatField("Random Place Angle", m_angle);
                m_placeAmount = EditorGUILayout.IntField("Place Amount", m_placeAmount);
            }
			


			if(customOffset == false)
			{
				if(enableHelp)
				{
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					EditorGUILayout.HelpBox("Click the Enable Custom Offset button below to Enable Custom Offset. This will allow you to adjust the x, y, and z of the position when added to the scene.", MessageType.Info);
				}

				if(GUILayout.Button("Enable Custom Offset"))
				{
					customOffset = true;
				}

				offSet = Vector3.zero;
			}
			else if(customOffset == true)
			{
				if(enableHelp)
				{
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					EditorGUILayout.HelpBox("Click the Disable Custom Offset button below to Disable Custom Offset.", MessageType.Info);
				}

				if(GUILayout.Button("Disable Custom Offset"))
				{
					customOffset = false;
				}
				if(enableHelp)
				{
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					EditorGUILayout.HelpBox("X, Y, Z values that will be added to the clicked position in the scene.", MessageType.Info);
				}

				offSet = EditorGUILayout.Vector3Field ("Set Offset", offSet);
			}


            // MUZ HACKS


            if (randomiseScale == false)
            {
                if (enableHelp)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Click the Enable Randomised Scale button below to Enable Randomised Scale. This will allow you to give a min and max scale range.", MessageType.Info);
                }

                if (GUILayout.Button("Enable Randomised Scale"))
                {
                    randomiseScale = true;
                }

            }
            else if (randomiseScale == true)
            {
                if (enableHelp)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Click the Disable Random Scale button below to Disable Random Scale.", MessageType.Info);
                }

                if (GUILayout.Button("Disable Random Scale"))
                {
                    //scaleMin = 1f;
                    //scaleMax = 1f;
                }
                if (enableHelp)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Set the Min and Max scale you want placed objects to be between.", MessageType.Info);
                }

                scaleMin = EditorGUILayout.Slider("Min: ", scaleMin, 0, 10);
                scaleMax = EditorGUILayout.Slider("Max: ", scaleMax, 0, 10);
            }


            // END MUZ HACKS



            // CREATE NEW GROUP
            EditorGUILayout.Space();
			EditorGUILayout.Space();

			if(enableHelp)
			{
				EditorGUILayout.HelpBox("You can create a new group to add your new objects when added to the scene. Just type the name you want the new group to be named then click the Create New Group Button" + 
				                        "all new objects added to the scene will be added to the new group", MessageType.Info);
			}

			newGroupName = EditorGUILayout.TextField("New Group Name:", newGroupName);

			if(GUILayout.Button("Create New Group"))
			{
				GameObject newObj = new GameObject();
				selectedGroup = newObj;
				newObj.name = newGroupName;
				newGroupName = "";
				GUIUtility.keyboardControl = 0;

				Undo.RegisterCreatedObjectUndo(newObj, "Created New Group " + newObj.name);
			}

			EditorGUILayout.Space();

			if(selectedGroup != null)
			{
				EditorGUILayout.LabelField("Selected Group: " + selectedGroup.name);
			}

			if(enableHelp)
			{
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("You can select an object in the scene to use for your group. Just select the object then click the Select Group button below to use this object for your group.", MessageType.Info);
			}
			if(GUILayout.Button("Select Group"))
			{
				if(Selection.activeGameObject != null)// && Selection.activeTransform != null)
				{
					selectedGroup = Selection.activeGameObject;
				}
			}

			if(enableHelp)
			{
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Click the Clear Selected Group button below to clear the selected group.", MessageType.Info);
			}
			if(GUILayout.Button("Clear Selected Group"))
			{
				selectedGroup = null;
			}

			if(enableHelp)
			{
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Enable Show Preview to see a preview of the object you have selected to add to the scene.", MessageType.Info);
			}

			if(showPreview == false)
			{
				showPreview = EditorGUILayout.Toggle ("Show Preview", showPreview);
			}
			else if(showPreview == true)
			{
				showPreview = EditorGUILayout.Toggle ("Hide Preview", showPreview);
				if(newSelectedGameObject != null)
				{
					GUILayout.Box(AssetPreview.GetAssetPreview(newSelectedGameObject), GUILayout.Width(128), GUILayout.Height(128));
				}
				if(newSelectedGameObject != null)
				{
					EditorGUILayout.LabelField("Selected GameObject: " + newSelectedGameObject.name);
				}
			}
		}

		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}


	void OnInspectorUpdate()
	{
		Repaint();
	}

}
