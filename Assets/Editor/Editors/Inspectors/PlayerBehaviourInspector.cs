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
        _target.speed = EditorGUILayout.FloatField("Speed", _target.speed);
        _target.targetDistance = EditorGUILayout.FloatField("Target Distance", _target.targetDistance);
        _target.gravitySpeed = EditorGUILayout.FloatField("Gravity Speed", _target.gravitySpeed);

        _target.move = EditorGUILayout.Vector3Field("Move Direction", _target.move);
        GUI.enabled = false;
        EditorGUILayout.Vector3Field("Look Target", _target.target);
        GUI.enabled = true;
    }

    private void HealthInspector() {
        _target.maxHP = EditorGUILayout.IntField("Max HP", _target.maxHP);
        _target.currentHP = EditorGUILayout.IntSlider("Current HP", _target.currentHP, 0, _target.maxHP);
    }

    private void DebugInspector() {
        DrawDefaultInspector();
    }

}
