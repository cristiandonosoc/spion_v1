using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class RoomBehaviourEditor {
    private void OnSceneGUI() {
        _target = (RoomBehaviour)target;
        ShowPosition();
        ShowLowerDim();
        ShowHigherDim();
    }

    private bool ShowPosition() {
        bool change = false;
        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.DoPositionHandle(_target.transform.position,
                                                  _target.transform.rotation);
        if (EditorGUI.EndChangeCheck()) {
            if (_target.snapVectors) {
                VectorHelpers.SnapVector(ref newPos, snappingFactor: 0.5f);
            }
            _target.transform.position = newPos;
            change = true;
        }
        return change;
    }

    private bool ShowLowerDim() {
        bool change = false;
        EditorGUI.BeginChangeCheck();
        Vector3 lowPos = _target.transform.position - _target.Extents;
        lowPos = VectorHelpers.RotateAroundPivot(lowPos, _target.transform.position,
                                                 _target.transform.rotation);
        Vector3 newPos = Handles.DoPositionHandle(lowPos, _target.transform.rotation);
        if (EditorGUI.EndChangeCheck()) {
            var rot = _target.transform.rotation.eulerAngles;
            newPos = VectorHelpers.RotateAroundPivot(newPos, _target.transform.position,
                                                     Quaternion.Euler(-rot));
            // We see how much this point moved to determine the new size
            // Position difference is inverted because we count negatively from position
            Vector3 posDiff = -(newPos - lowPos);
            Vector3 newSize = _target.Size + posDiff / 2;
            if (_target.snapVectors) {
                VectorHelpers.SnapVector(ref newSize, snappingFactor: 0.5f);
            }
            // We see the size difference and move the position accordingly
            Vector3 sizeDiff = newSize - _target.Size;
            // We substract because size changes invert the change on position
            _target.transform.localPosition -= sizeDiff / 2;
            _target.Size = newSize;
            change = true;
        }
        return change;
    }

    private bool ShowHigherDim() {
        bool change = false;
        EditorGUI.BeginChangeCheck();
        Vector3 highPos = _target.transform.position + _target.Extents;
        highPos = VectorHelpers.RotateAroundPivot(highPos, _target.transform.position,
                                                  _target.transform.rotation);
        Vector3 newPos = Handles.DoPositionHandle(highPos, _target.transform.rotation);
        if (EditorGUI.EndChangeCheck()) {
            var rot = _target.transform.rotation.eulerAngles;
            newPos = VectorHelpers.RotateAroundPivot(newPos, _target.transform.position,
                                                     Quaternion.Euler(-rot));

            // We see how much this point moved to determine the new size
            Vector3 posDiff = newPos - highPos;
            Vector3 newSize = _target.Size + posDiff / 2;
            if (_target.snapVectors) {
                VectorHelpers.SnapVector(ref newSize, snappingFactor: 0.5f);
            }
            // We see the size difference and move the position accordingly
            Vector3 sizeDiff = newSize - _target.Size;
            _target.transform.localPosition += sizeDiff / 2;
            _target.Size = newSize;
            change = true;
        }
        return change;
    }




}
