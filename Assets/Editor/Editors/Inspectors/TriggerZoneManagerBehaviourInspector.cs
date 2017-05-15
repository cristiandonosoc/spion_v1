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

    private void GetCurrentTriggerOptions(out List<SerializableType> bindedTypes,
                                          out List<SerializableType> messageKindTypes,
                                          out List<int> messageKindValues,
                                          out List<string> optionNames) {
        bindedTypes = new List<SerializableType>();
        messageKindTypes = new List<SerializableType>();
        messageKindValues = new List<int>();
        optionNames = new List<string>();

        // We add the default non binded option
        bindedTypes.Add(null);
        messageKindTypes.Add(null);
        messageKindValues.Add(-1);
        optionNames.Add("NO BINDING");

        CustomMonoBehaviour[] components = _target.GetComponents<CustomMonoBehaviour>();
        foreach (CustomMonoBehaviour component in components) {
            Type componentType = component.GetType();

            foreach (Type nestedType in componentType.GetNestedTypes(BindingFlags.Public)) {
                if (nestedType.GetCustomAttributes(typeof(MessageKindMarker), false).Length > 0) {
                    Array enumValues = Enum.GetValues(nestedType);
                    string[] enumNames = Enum.GetNames(nestedType);

                    for (int i = 0; i < enumValues.Length; i++) {
                        // We get the type
                        bindedTypes.Add(componentType);
                        messageKindTypes.Add(nestedType);

                        int enumValue = (int)enumValues.GetValue(i);
                        messageKindValues.Add(enumValue);

                        string enumName = enumNames[i];
                        string optionName = componentType.ToString() + " | " + enumName;
                        optionNames.Add(optionName);
                    }
                }
            }
        }
    }

    private void ZonesInspector() {
        // We get the needed data for making the triggerZone association
        List<SerializableType> bindedTypes;
        List<SerializableType> messageKindTypes;
        List<int> messageKindValues;
        List<string> optionNames;
        GetCurrentTriggerOptions(out bindedTypes, out messageKindTypes, out messageKindValues, out optionNames);

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

            TriggerZoneBehaviour[] triggerZones = _target.GetComponentsInChildren<TriggerZoneBehaviour>();
            List<TriggerZoneBinding> triggerZoneMappings = _target.TriggerZoneMappings;

            List<TriggerZoneBinding> updatedTriggerZoneMappings = new List<TriggerZoneBinding>();

            // We match the trigger zones
            bool found = false;
            foreach (TriggerZoneBehaviour triggerZone in triggerZones) {
                // We search for a trigger mapping with the same name
                foreach (TriggerZoneBinding triggerZoneMapping in triggerZoneMappings) {
                    // Matching is by name
                    if (triggerZoneMapping.TriggerZone.name == triggerZone.name) {
                        updatedTriggerZoneMappings.Add(triggerZoneMapping);
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    // We add a new TriggerZone
                    updatedTriggerZoneMappings.Add(new TriggerZoneBinding(triggerZone));
                }
            }

            // We match the options
            foreach (TriggerZoneBinding mapping in updatedTriggerZoneMappings) {
                // TODO(Cristian): This could be more efficient
                int enterIndex = 0;
                int exitIndex = 0;
                for (int i = 0; i < optionNames.Count; i++) {
                    string methodName = optionNames[i];
                    if (mapping.EnterKey == methodName) {
                        enterIndex = i;
                    }
                    if (mapping.ExitKey == methodName) {
                        exitIndex = i;
                    }
                }

                string[] optionNameArray = optionNames.ToArray();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(mapping.TriggerZone.name, GUILayout.Width(elementWidth));

                enterIndex = EditorGUILayout.Popup(enterIndex, optionNameArray, GUILayout.Width(elementWidth));
                if (enterIndex != 0) {
                    mapping.EnterBindedType = bindedTypes[enterIndex];
                    mapping.EnterKey = optionNames[enterIndex];
                    Type enterMessageType = messageKindTypes[enterIndex].type;
                    Message enterMessage = new Message(enterMessageType, messageKindValues[enterIndex], null);
                    mapping.EnterInternalMessage = enterMessage;
                }

                exitIndex = EditorGUILayout.Popup(exitIndex, optionNameArray);
                if (exitIndex != 0) {
                    mapping.ExitBindedType = bindedTypes[exitIndex];
                    mapping.ExitKey = optionNames[exitIndex];
                    Type exitMessageType = messageKindTypes[exitIndex].type;
                    Message exitMessage = new Message(exitMessageType, messageKindValues[exitIndex], null);
                    mapping.ExitInternalMessage = exitMessage;
                }

                EditorGUILayout.EndHorizontal();
            }

            _target.TriggerZoneMappings = updatedTriggerZoneMappings;
            EditorGUI.indentLevel--;
        }

        if (GUILayout.Button("Add Trigger Zone")) {
            TriggerZoneBehaviour zone = _target.AddTriggerZone();
            Selection.activeTransform = zone.transform;
        }
    }

    private void CreateMappingPopups(ref List<TriggerZoneBinding> triggerZoneMappings) {

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
