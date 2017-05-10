using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggerZoneManagerBehaviour))]
public class TriggerZoneManagerBehaviourEditor : SpecializedInspector {

    private TriggerZoneManagerBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (TriggerZoneManagerBehaviour)target;

        IndentedInspector("Zones", ZonesInspector);
        HorizontalBreak();
        IndentedInspector("Debug", DebugInspector);
    }

    public bool mappingFoldoutOpen = true;

    private void ZonesInspector() {

        List<string> methodNames = new List<string>();
        CustomMonoBehaviour[] components = _target.GetComponents<CustomMonoBehaviour>();

        foreach (CustomMonoBehaviour component in components) {
            Type t = component.GetType();
            MethodInfo[] methods = t.GetMethods();
            foreach (MethodInfo methodInfo in methods) {
                if (IsMethodCompatibleWithDeletagate<TriggerZoneDelegate>(methodInfo)) {
                    methodNames.Add(methodInfo.ToString());
                }
            }
        }



        mappingFoldoutOpen = EditorGUILayout.Foldout(mappingFoldoutOpen, "Mapping");
        if (mappingFoldoutOpen) {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Zone Name");
            EditorGUILayout.LabelField("Enter Trigger");
            EditorGUILayout.LabelField("Exit Trigger");
            EditorGUILayout.EndHorizontal();


            foreach (TriggerMapping mapping in _target.TriggerMappings) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(mapping.Key);
                EditorGUILayout.Popup(0, methodNames.ToArray());
                EditorGUILayout.Popup(0, methodNames.ToArray());
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }

        if (GUILayout.Button("Add Trigger Zone")) {
            TriggerZoneBehaviour zone = _target.AddTriggerZone();
            Selection.activeTransform = zone.transform;
        }
    }

    private void DebugInspector() {
        DrawDefaultInspector();
    }

    public bool IsMethodCompatibleWithDeletagate<T>(MethodInfo methodInfo) {
        Type delegateType = typeof(T);
        MethodInfo delegateSignature = delegateType.GetMethod("Invoke");

        ParameterInfo[] delegateParameters = delegateSignature.GetParameters();
        ParameterInfo[] methodParameters = methodInfo.GetParameters();

        if (delegateParameters.Length != methodParameters.Length) { return false; }

        for (int i = 0; i < delegateParameters.Length; i++) {
            if (delegateParameters[i].ParameterType != methodParameters[i].ParameterType) { return false; }
        }

        if (delegateSignature.ReturnType != methodInfo.ReturnType) { return false; }
        return true;
    }

}
