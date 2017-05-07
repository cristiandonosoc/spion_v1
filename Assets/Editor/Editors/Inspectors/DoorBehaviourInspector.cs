using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorBehaviour))]
public partial class DoorBehaviourEditor : SpecializedInspector {

    private DoorBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (DoorBehaviour)target;

        IndentedInspector("Debug", DebugInspector);
    }

    private void DebugInspector() {
        DrawDefaultInspector();
        if (GUILayout.Button("Refresh")) {
            _target.Refresh();
        }
    }

}
