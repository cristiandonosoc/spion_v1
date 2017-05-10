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
        methodNames.Add("");
        CustomMonoBehaviour[] components = _target.GetComponents<CustomMonoBehaviour>();

        foreach (CustomMonoBehaviour component in components) {
            Type t = component.GetType();
            MethodInfo[] methods = t.GetMethods();
            foreach (MethodInfo methodInfo in methods) {
                if (IsMethodCompatibleWithDeletagate<TriggerZoneDelegate>(methodInfo)) {
                    methodNames.Add(t.ToString() + "|" + methodInfo.Name);
                }
            }
        }



        mappingFoldoutOpen = EditorGUILayout.Foldout(mappingFoldoutOpen, "Mapping");
        if (mappingFoldoutOpen) {
            EditorGUI.indentLevel++;

            float layoutWidth = InspectorHelpers.GetCurrentLayoutWidth();
            int elementCount = 3;
            float elementWidth = layoutWidth / elementCount;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Trigger Name", EditorStyles.boldLabel, GUILayout.Width(elementWidth));
            EditorGUILayout.LabelField("On Enter", EditorStyles.boldLabel, GUILayout.Width(elementWidth));
            EditorGUILayout.LabelField("On Exit", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            foreach (TriggerMapping mapping in _target.TriggerMappings) {
                // TODO(Cristian): This could be more efficient
                int enterIndex = 0;
                int exitIndex = 0;
                for (int i = 0; i < methodNames.Count; i++) {
                    string methodName = methodNames[i];
                    if (mapping.EnterTrigger == methodName) {
                        enterIndex = i;
                    }
                    if (mapping.ExitTrigger == methodName) {
                        exitIndex = i;
                    }
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(mapping.Key, GUILayout.Width(elementWidth));
                enterIndex = EditorGUILayout.Popup(enterIndex, methodNames.ToArray(), GUILayout.Width(elementWidth));
                mapping.EnterTrigger = methodNames[enterIndex];
                exitIndex = EditorGUILayout.Popup(exitIndex, methodNames.ToArray());
                mapping.ExitTrigger = methodNames[exitIndex];
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
        if (GUILayout.Button("Refresh Trigger Zones")) {
            _target.RefreshTriggerZones();
        }
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
