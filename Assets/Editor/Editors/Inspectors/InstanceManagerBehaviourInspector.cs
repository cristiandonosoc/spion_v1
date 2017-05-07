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
        IndentedInspector("Defaults", DefaultsInspector);
    }

    private void ActionsInspector() {
        if (GUILayout.Button("Add Room")) {
            RoomBehaviour room = _target.AddRoom();
            Selection.activeTransform = room.transform;
        }
    }

    private void DefaultsInspector() {
        _target.defaultBlockModel = (ModelBehaviour)EditorGUILayout.ObjectField("Default Block Prefab",
                                                                                _target.defaultBlockModel,
                                                                                typeof(ModelBehaviour),
                                                                                false);
    }

}
