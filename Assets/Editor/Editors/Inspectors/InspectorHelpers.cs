using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

}
