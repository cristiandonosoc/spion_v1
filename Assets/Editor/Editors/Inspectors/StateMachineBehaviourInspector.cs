using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StateMachineBehaviour))]
public class StateMachineBehaviourInspector : SpecializedInspector {
    private StateMachineBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (StateMachineBehaviour)target;

        IndentedInspector("States", StateInspector);
        HorizontalBreak();
        IndentedInspector("Debug", DebugInspector, openAtStart: false);
    }

    Type[] enumTypes;
    string[] enumNames;

    int[] stateValues;
    string[] stateNames;
    Dictionary<int, string> stateValueNameMap;

    private void RefreshData() {
        enumTypes = TypeHelpers.GetAttributedTypes(typeof(StateMachineEnum));
        enumNames = TypeHelpers.GetNames(enumTypes);

        if (_target.StateEnumType != null) {
            stateValues = TypeHelpers.GetEnumValues(_target.StateEnumType);
            stateNames = Enum.GetNames(_target.StateEnumType);
            stateValueNameMap = TypeHelpers.GetEnumValueNameMap(_target.StateEnumType);
        }
    }


    private void StateInspector() {
        if (GUILayout.Button("Refresh State Machine Data") || (enumTypes == null)) {
            RefreshData();
        }

        _target.Name = EditorGUILayout.TextField("State Machine Name", _target.Name);

        int currentStateEnumIndex = TypeHelpers.GetIndexInArray(_target.StateEnumType, enumTypes);
        int newStateEnumIndex = EditorGUILayout.Popup("State Machine Enum", currentStateEnumIndex, enumNames);
        if (newStateEnumIndex != currentStateEnumIndex) {
            if ((currentStateEnumIndex == -1) ||
                EditorUtility.DisplayDialog("State Machine Enum Change",
                "This will erase all the transitions associated with this state machine.\n" +
                "Think if this is what you want. If you don't know, press Cancel",
                "Erase It", "Cancel")) {
                _target.SetType(enumTypes[newStateEnumIndex]);
                RefreshData();
            } else {
                newStateEnumIndex = currentStateEnumIndex;
            }
        }


        if (newStateEnumIndex == -1) {
            EditorGUILayout.HelpBox("Please select a state", MessageType.Info);
        } else {
            int currentStateIndex = TypeHelpers.GetIndexInArray(_target.CurrentInternalStateRepresentation, stateValues);
            int newStateIndex = EditorGUILayout.Popup("Current State", currentStateIndex, stateNames);
            if (newStateIndex != currentStateIndex) {
                _target.CurrentInternalStateRepresentation = stateValues[newStateIndex];
            }

            IndentedInspector("Transitions", TransitionsInspector);
        }
    }

    private void TransitionsInspector() {

        if (stateValues == null) {
            RefreshData();
        }

        // Titles
        InspectorHelpers.CreateRow(new string[] { "STATE", "TRANSITIONS", "ADD" }, EditorStyles.boldLabel);


        float layoutWidth = InspectorHelpers.GetCurrentLayoutWidth();
        int elementCount = 3;
        float elementWidth = layoutWidth / elementCount - 3;
        GUILayoutOption widthOption = GUILayout.Width(elementWidth);

        for (int i = 0; i < stateValues.Length; i++) {
            int state = stateValues[i];
            StateTransitions stateTransitions = _target.GetStateTransitionsForState(state);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(stateNames[i], widthOption);
                EditorGUILayout.BeginVertical(widthOption);
                {
                    if (stateTransitions.transitions.Count == 0) {
                        EditorGUILayout.LabelField("NO TRANSITION", widthOption);
                    } else {
                        // We can only click one button per time... right?
                        int transitionToRemove = -1;
                        foreach (int transition in stateTransitions.transitions) {
                            EditorGUILayout.BeginHorizontal(widthOption);
                            float halfWidth = elementWidth / 2 - 3;
                            EditorGUILayout.LabelField(stateValueNameMap[transition], GUILayout.Width(halfWidth));
                            if (GUILayout.Button("Remove", GUILayout.Width(halfWidth))) {
                                transitionToRemove = transition;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (transitionToRemove != -1) {
                            stateTransitions.RemoveTransition(transitionToRemove);
                        }
                    }
                }
                EditorGUILayout.EndVertical();

                int newIndex = EditorGUILayout.Popup(-1, stateNames, widthOption);
                if (newIndex != -1) {
                    stateTransitions.AddTransition(stateValues[newIndex]);
                }
            }
            EditorGUILayout.EndHorizontal();
        }



    }

    private void DebugInspector() {
        DrawDefaultInspector();
    }

}
