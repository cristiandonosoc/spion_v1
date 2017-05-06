using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class RoomBehaviourEditor {
    private void OnSceneGUI() {
        _roomBehaviour = (RoomBehaviour)target;
        ShowPosition();
        ShowLowerDim();
        ShowHigherDim();
    }

    private bool ShowPosition() {
        bool change = false;
        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.DoPositionHandle(_roomBehaviour.transform.position,
                                                  _roomBehaviour.transform.rotation);
        if (EditorGUI.EndChangeCheck()) {
            if (_roomBehaviour.snapVectors) {
                VectorHelpers.SnapVector(ref newPos, snappingFactor: 0.5f);
            }
            _roomBehaviour.transform.position = newPos;
            change = true;
        }
        return change;
    }

    private bool ShowLowerDim() {
        bool change = false;
        EditorGUI.BeginChangeCheck();
        Vector3 lowPos = _roomBehaviour.transform.position - _roomBehaviour.Extents;
        lowPos = VectorHelpers.RotateAroundPivot(lowPos, _roomBehaviour.transform.position,
                                                 _roomBehaviour.transform.rotation);
        Vector3 newPos = Handles.DoPositionHandle(lowPos, _roomBehaviour.transform.rotation);
        if (EditorGUI.EndChangeCheck()) {
            var rot = _roomBehaviour.transform.rotation.eulerAngles;
            newPos = VectorHelpers.RotateAroundPivot(newPos, _roomBehaviour.transform.position,
                                                     Quaternion.Euler(-rot));
            Vector3 size = (newPos - _roomBehaviour.transform.position) * 2;
            if (_roomBehaviour.snapVectors) {
                VectorHelpers.SnapVector(ref size);
            }
            size = -size;   // This is lower
            _roomBehaviour.Size = size;
            change = true;
        }
        return change;
    }



    private bool ShowHigherDim() {
        bool change = false;
        EditorGUI.BeginChangeCheck();
        Vector3 highPos = _roomBehaviour.transform.position + _roomBehaviour.Extents;
        highPos = VectorHelpers.RotateAroundPivot(highPos, _roomBehaviour.transform.position,
                                                  _roomBehaviour.transform.rotation);
        Vector3 newPos = Handles.DoPositionHandle(highPos, _roomBehaviour.transform.rotation);
        if (EditorGUI.EndChangeCheck()) {
            var rot = _roomBehaviour.transform.rotation.eulerAngles;
            newPos = VectorHelpers.RotateAroundPivot(newPos, _roomBehaviour.transform.position,
                                                     Quaternion.Euler(-rot));
            Vector3 size = (newPos - _roomBehaviour.transform.position) * 2;
            if (_roomBehaviour.snapVectors) {
                VectorHelpers.SnapVector(ref size);
            }
            _roomBehaviour.Size = size;
            change = true;
        }
        return change;
    }

}
