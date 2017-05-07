using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InstanceManagerBehaviour))]
public partial class InstanceManagerBehaviourEditor : SpecializedInspector {
    private InstanceManagerBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (InstanceManagerBehaviour)target;

        IndentedInspector("Room", ActionsInspector);
        HorizontalBreak();
        IndentedInspector("Debug", DebugInspector);
    }

    private void ActionsInspector() {
        if (GUILayout.Button("Add Room")) {
            RoomBehaviour room = _target.AddRoom();
            Selection.activeTransform = room.transform;
        }
    }

    private void DebugInspector() {
        DrawDefaultInspector();
    }

}
