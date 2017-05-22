using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerBehaviour))]
public partial class PlayerBehaviourInspector : SpecializedInspector {
    private PlayerBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (PlayerBehaviour)target;

        IndentedInspector("Movement", MovementInspector);
        HorizontalBreak();
        IndentedInspector("Health", HealthInspector);
        HorizontalBreak();
        IndentedInspector("Debug", DebugInspector);
    }

    private void MovementInspector() {
        _target.Speed = EditorGUILayout.FloatField("Speed", _target.Speed);
        _target.TargetDistance = EditorGUILayout.FloatField("Target Distance", _target.TargetDistance);
        _target.GravitySpeed = EditorGUILayout.FloatField("Gravity Speed", _target.GravitySpeed);

        GUI.enabled = false;
        EditorGUILayout.Vector3Field("Move Direction", _target.MoveDirection);
        EditorGUILayout.Vector3Field("Look Target", _target.TargetPosition);
        GUI.enabled = true;
    }

    private void HealthInspector() {
        _target.MaxHP = EditorGUILayout.IntField("Max HP", _target.MaxHP);
        _target.CurrentHP = EditorGUILayout.IntSlider("Current HP", _target.CurrentHP, 0, _target.MaxHP);
    }

    private void DebugInspector() {
        DrawDefaultInspector();
    }

}
