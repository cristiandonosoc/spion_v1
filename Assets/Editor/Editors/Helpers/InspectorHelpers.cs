using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class InspectorHelpers {

    public const float pixelsPerIndentLevel = 9;

    /// <summary>
    /// Will get the current rect used by the UI.
    /// Useful for when using indented inspectors.
    /// Currently getting it by an awful hack, but haven't
    /// found a better way to do this (Cristian)
    /// </summary>
    /// <returns>Rect with current layout dimensions</returns>
    public static float GetCurrentLayoutWidth() {
        return Screen.width - pixelsPerIndentLevel * EditorGUI.indentLevel;
    }

    public static void CreateRow(string[] labels, GUIStyle style = null, params GUILayoutOption[] GUIOptions) {
        float layoutWidth = InspectorHelpers.GetCurrentLayoutWidth();
        float elementWidth = layoutWidth / labels.Length;

        // We need to create another one to forward the options
        GUILayoutOption[] forwardOptions = new GUILayoutOption[GUIOptions.Length + 1];
        Array.Copy(GUIOptions, forwardOptions, GUIOptions.Length);
        forwardOptions[forwardOptions.Length - 1] = GUILayout.Width(elementWidth);

        EditorGUILayout.BeginHorizontal();
        foreach (string label in labels) {
            EditorGUILayout.LabelField(label, style, forwardOptions);
        }
        EditorGUILayout.EndHorizontal();
    }

}
