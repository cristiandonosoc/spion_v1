using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomBehaviour))]
public partial class RoomBehaviourEditor : SpecializedInspector {

    private RoomBehaviour _roomBehaviour;

    public override void OnInspectorGUI() {
        _roomBehaviour = (RoomBehaviour)target;

        IndentedInspector("Actions", ActionsInspector);
        HorizontalBreak();
        IndentedInspector("Data", DataInspector);
        HorizontalBreak();
        IndentedInspector("Misc", MiscInspector);
        HorizontalBreak();
        IndentedInspector("Debug", DebugInspector);
    }

    private void ActionsInspector() {
        //if (GUILayout.Button("Add Floor")) {
        //    _roomBehaviour.AddFloor();
        //}
        //if (GUILayout.Button("Add Wall")) {
        //    WallBehaviour wall = _roomBehaviour.AddWall();
        //    Selection.activeTransform = wall.transform;
        //}
        //if (GUILayout.Button("Add Door")) {
        //    DoorBehaviour door = _roomBehaviour.AddDoor();
        //    Selection.activeTransform = door.transform;
        //}

        if (GUILayout.Button("Add Block")) {
            BlockBehaviour block = _roomBehaviour.AddBlock();
            Selection.activeTransform = block.transform;
        }
    }

    private void DataInspector() {
        EditorGUI.BeginChangeCheck();
        Vector3 size = EditorGUILayout.Vector3Field("Size", _roomBehaviour.Size);
        if (EditorGUI.EndChangeCheck()) {
            if (_roomBehaviour.snapVectors) {
                VectorHelpers.SnapVector(ref size);
            }
            _roomBehaviour.Size = size;
        }
    }

    private void MiscInspector() {
        _roomBehaviour.snapVectors = EditorGUILayout.Toggle("Snap Vectors", _roomBehaviour.snapVectors);
    }

    private void DebugInspector() {
        DrawDefaultInspector();
    }
}