using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerBehaviour))]
public partial class PlayerBehaviourInspector : SpecializedInspector {
    private PlayerBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (PlayerBehaviour)target;

        IndentedInspector("Normal", NormalInspector);
        HorizontalBreak();
        IndentedInspector("Dashing", DashingInspector);
        HorizontalBreak();
        IndentedInspector("Health", HealthInspector);
        HorizontalBreak();
        IndentedInspector("Debug", DebugInspector, openAtStart: false);
    }

    private void NormalInspector() {
        _target.InternalMoveData.speed = EditorGUILayout.FloatField("Speed", _target.InternalMoveData.speed);

        _target.TargetDistance = EditorGUILayout.FloatField("Target Distance", _target.TargetDistance);
        _target.GravitySpeed = EditorGUILayout.FloatField("Gravity Speed", _target.GravitySpeed);

        GUI.enabled = false;
        EditorGUILayout.Vector3Field("Move Direction", _target.MoveDirection);
        EditorGUILayout.Vector3Field("Look Target", _target.TargetPosition);
        GUI.enabled = true;
    }

    private void DashingInspector() {
        _target.InternalDashingData.speed = EditorGUILayout.FloatField("Speed", _target.InternalDashingData.speed);
        _target.InternalDashingData.currentDuration = EditorGUILayout.FloatField("Current Duration", 
                                                                                 _target.InternalDashingData.currentDuration);
        _target.InternalDashingData.duration = EditorGUILayout.FloatField("Duration", 
                                                                          _target.InternalDashingData.duration);

    }

    private void HealthInspector() {

        var hc = (HealthComponentBehaviour)EditorGUILayout.ObjectField("Health Component", 
                                                                                        _target.HealthComponent,
                                                                                        typeof(HealthComponentBehaviour), 
                                                                                        allowSceneObjects: true);
        _target.HealthComponent = hc;

        if (hc == null) { return; }

        hc.MaxHP = EditorGUILayout.IntField("Max HP", hc.MaxHP);
        hc.CurrentHP = EditorGUILayout.IntSlider("Current HP", hc.CurrentHP, 0, hc.MaxHP);
    }

    private void DebugInspector() {
        DrawDefaultInspector();
    }

}
