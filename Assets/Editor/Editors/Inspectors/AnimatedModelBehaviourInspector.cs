using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimatedModelBehaviour))]
public class AnimatedModelBehaviourEditor : SpecializedInspector {

    private ModelBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (ModelBehaviour)target;

        IndentedInspector("Debug", DebugInspector);
    }

    private void DebugInspector() {
        DrawDefaultInspector();
        if (GUILayout.Button("Log Mesh Size")) {
            Vector3 meshSize = _target.MeshSize;
            Debug.Log(meshSize);
        }
    }

}
