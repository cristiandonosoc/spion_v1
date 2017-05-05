using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class BlockBehaviourEditor { 

    private void OnSceneGUI() {
        _target = (BlockBehaviour)target;

        ShowPosition();

        bool change = false;
        change |= ShowLowerDim();
        change |= ShowHigherDim();

        if (change) {
            _target.Refresh();
        }
    }

    private bool ShowPosition() {
        bool change = false;
        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.DoPositionHandle(_target.transform.position,
                                                  _target.transform.rotation);
        if (EditorGUI.EndChangeCheck()) {
            if (_target.snapVectors) {
                VectorHelpers.SnapVector(ref newPos);
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
            Vector3 size = (newPos - _target.transform.position) * 2;
            if (_target.snapVectors) {
                VectorHelpers.SnapVector(ref size);
            }
            size = -size;   // This is lower
            _target.Size = size;
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
            Vector3 size = (newPos - _target.transform.position) * 2;
            if (_target.snapVectors) {
                VectorHelpers.SnapVector(ref size);
            }
            _target.Size = size;
            change = true;
        }
        return change;
    }



}
