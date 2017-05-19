using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(AnimationState))]
public class AnimationStateInspector : SpecializedInspector{

    public const string stateEnumLabel = "State Enum";
    public const string stateEnumTooltip = "This is the state machine to use in this state.\n Should be the same throughout the animator";
    public const string inStateEnumLabel = "Enum Value";
    public const string inStateEnumTooltip = "The actual value that this state should have.\n This is the one the animated object will query";

    private AnimationState _target;

    public override void OnInspectorGUI() {
        _target = (AnimationState)target;

        IndentedInspector("State", StateInspector);
        HorizontalBreak();
        IndentedInspector("Debug", DebugInspector);
    }

    private void StateInspector() {

        // We get the enum types
        Type[] enumTypes = TypeHelpers.GetAttributedTypes(typeof(AnimationEnum));

        // We get the strings end index
        int currentStateEnumIndex = 0;
        GUIContent[] enumNames = new GUIContent[enumTypes.Length];
        for (int i = 0; i < enumTypes.Length; i++) {
            enumNames[i] = new GUIContent(enumTypes[i].ToString());
            if (_target.enumType == enumTypes[i]) {
                currentStateEnumIndex = i;
            }
        }

        currentStateEnumIndex = EditorGUILayout.Popup(new GUIContent(stateEnumLabel, stateEnumTooltip), 
                                                      currentStateEnumIndex, enumNames);
        _target.enumType = enumTypes[currentStateEnumIndex];


        // Now we get the states for the current enum type
        int currentInEnumIndex = 0;
        Array enumValues = Enum.GetValues(_target.enumType.type);
        string[] enumOptionNames = Enum.GetNames(_target.enumType.type);
        GUIContent[] enumOptionLabels = new GUIContent[enumValues.Length];
        for (int i = 0; i < enumValues.Length; i++) {
            enumOptionLabels[i] = new GUIContent(enumOptionNames[i]);

            int value = (int)enumValues.GetValue(i);
            if (_target.enumValue == value) {
                currentInEnumIndex = i;
            }
        }
        currentInEnumIndex = EditorGUILayout.Popup(new GUIContent(inStateEnumLabel, inStateEnumTooltip), 
                                                   currentInEnumIndex, enumOptionLabels);
        _target.enumValue = (int)enumValues.GetValue(currentInEnumIndex);


    }

    private void DebugInspector() {
        DrawDefaultInspector();
    }

}
