using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorBehaviour))]
public partial class DoorBehaviourEditor : SpecializedInspector {

    private DoorBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (DoorBehaviour)target;


        IndentedInspector("Actions", ActionsInspector);
        HorizontalBreak();
        IndentedInspector("Debug", DebugInspector);
    }

    private void ActionsInspector() {
        if (Application.isPlaying) {
            if (GUILayout.Button("Open Door")) {
                _target.Open();
            }
            if (GUILayout.Button("Close Door")) {
                _target.Close();
            }
        }
    }

    private void DebugInspector() {
        DrawDefaultInspector();
        if (GUILayout.Button("Refresh")) {
            _target.Refresh();
        }
    }

}
