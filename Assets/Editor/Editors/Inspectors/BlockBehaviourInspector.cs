using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockBehaviour))]
public partial class BlockBehaviourEditor : SpecializedInspector {

    private BlockBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (BlockBehaviour)target;

        IndentedInspector("Debug", DebugInspector);
    }

    private void DebugInspector() {
        DrawDefaultInspector();
        if (GUILayout.Button("Refresh")) {
            _target.Refresh();
        }
    }
}
