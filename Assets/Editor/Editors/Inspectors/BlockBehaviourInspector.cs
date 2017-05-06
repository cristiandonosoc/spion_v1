using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockBehaviour))]
public partial class BlockBehaviourEditor : SpecializedInspector {

    private BlockBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (BlockBehaviour)target;

        IndentedInspector("Snapping", SnappingInspector);
        HorizontalBreak();
        IndentedInspector("Debug", DebugInspector);
    }

    private void SnappingInspector() {
        if (GUILayout.Button("Snap to Floor")) {
            BlockHelper.SnapToFloor(_target);
        }

        int wallIndex = -1;
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Button("");
        if (GUILayout.Button("Snap North")) { wallIndex = 0; }
        GUILayout.Button("");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Snap West")) { wallIndex = 3; }
        GUILayout.Button("");
        if (GUILayout.Button("Snap East")) { wallIndex = 1; }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Button("");
        if (GUILayout.Button("Snap South")) { wallIndex = 2; }
        GUILayout.Button("");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        if (wallIndex != -1) {
            BlockHelper.SnapToRoom(_target, wallIndex);
        }
    }

    private void DebugInspector() {
        DrawDefaultInspector();
        if (GUILayout.Button("Refresh")) {
            _target.Refresh();
        }
    }
}
