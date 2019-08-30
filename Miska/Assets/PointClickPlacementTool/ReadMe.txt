Point Click Placement Tools
To open the dockable window goto GameObject\Point Click Placement Tool

Just click the enable button, select a gameobject or prefab from the project then left click anywhere in the scene where there is an existing collider.

For more information click the Enable Help for detailed information on each control.




NOTE FROM A USER:

My little suggestion: 
An option to automatically rotate the new prefab on Y axis (good for trees!).

I added this, next to if (useNormalRotation)...

		if (randomRotateYAxis) {
			prefab.transform.rotation = Quaternion.AngleAxis (Random.Range (0.0f, 360.0f), prefab.transform.up) * prefab.transform.rotation;
		}

And also 
randomRotateYAxis = EditorGUILayout.Toggle ("Random rotation on Y axis", randomRotateYAxis);

after the line useNormalRotation = EditorGUILayout.Toggle...

And of course created the bool randomRotateYAxis. 

Now I check this every time I place a new tree, so each has a different rotation, looking more natural.

