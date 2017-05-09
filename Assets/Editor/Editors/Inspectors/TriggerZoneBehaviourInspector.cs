using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggerZoneBehaviour))]
public partial class TriggerZoneBehaviourEditor : SpecializedInspector {
    private TriggerZoneBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (TriggerZoneBehaviour)target;

        IndentedInspector("Debug", DebugInspector);
    }

    private void DebugInspector() {
        DrawDefaultInspector();
        if (GUILayout.Button("Refresh")) {
            _target.Refresh();
        }
    }

}
