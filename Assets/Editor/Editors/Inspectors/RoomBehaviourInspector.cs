using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomBehaviour))]
public partial class RoomBehaviourEditor : SpecializedInspector {

    private RoomBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (RoomBehaviour)target;

        IndentedInspector("Actions", ActionsInspector);
        HorizontalBreak();
        IndentedInspector("Data", DataInspector);
        HorizontalBreak();
        IndentedInspector("Misc", MiscInspector);
        HorizontalBreak();
        IndentedInspector("Debug", DebugInspector);
    }

    private void ActionsInspector() {
        if (GUILayout.Button("Add Block")) {
            BlockBehaviour block = _target.AddBlock();
            Selection.activeTransform = block.transform;
        }

        if (GUILayout.Button("Add Door")) {
            DoorBehaviour door = _target.AddDoor();
            Selection.activeTransform = door.transform;
        }
    }

    private void DataInspector() {
        EditorGUI.BeginChangeCheck();
        Vector3 size = EditorGUILayout.Vector3Field("Size", _target.Size);
        if (EditorGUI.EndChangeCheck()) {
            if (_target.snapVectors) {
                VectorHelpers.SnapVector(ref size);
            }
            _target.Size = size;
        }
    }

    private void MiscInspector() {
        _target.snapVectors = EditorGUILayout.Toggle("Snap Vectors", _target.snapVectors);
    }

    private void DebugInspector() {
        DrawDefaultInspector();
    }
}