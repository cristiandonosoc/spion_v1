using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InstanceManagerBehaviour))]
public partial class InstanceManagerBehaviourEditor : SpecializedInspector {
    private InstanceManagerBehaviour _imb;

    public override void OnInspectorGUI() {
        _imb = (InstanceManagerBehaviour)target;

        IndentedInspector("Room", ActionsInspector);
    }

    private void ActionsInspector() {
        _imb.defaultFloorPanelCount = EditorGUILayout.Vector3Field("Default Floor Panel Count",
                                                                   _imb.defaultFloorPanelCount);


        _imb.defaultFloorPanel = (ModelBehaviour)EditorGUILayout.ObjectField("Default Floor Panel", 
                                                                             _imb.defaultFloorPanel,
                                                                             typeof(ModelBehaviour), false);
        _imb.createFloorWithRoom = EditorGUILayout.Toggle("Create Floor With Room",
                                                          _imb.createFloorWithRoom);

        _imb.defaultWallCount = EditorGUILayout.Vector3Field("Default Wall Count",
                                                             _imb.defaultWallCount);
        _imb.defaultWall = (ModelBehaviour)EditorGUILayout.ObjectField("Default Wall",
                                                                       _imb.defaultWall,
                                                                       typeof(ModelBehaviour), false);
        _imb.createWallsWithRoom = EditorGUILayout.Toggle("Create Walls With Room",
                                                          _imb.createWallsWithRoom);

        if (GUILayout.Button("Add Room")) {
            _imb.AddRoom();
        }
    }

}
