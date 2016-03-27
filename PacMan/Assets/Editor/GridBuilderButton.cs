using UnityEngine;
using System.Collections;
using UnityEditor;

//Places a button in a Grid script component that allows generation of the grid inside the Editor
[CustomEditor(typeof(GridManager))]
public class GridBuilderButton : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		GridManager myScript = (GridManager)target;
		if(GUILayout.Button("Build Grid")) {
			myScript.BuildGrid();
		}
	}
}
