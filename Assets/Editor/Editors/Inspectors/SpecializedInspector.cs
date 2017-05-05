using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpecializedInspector : Editor {
    protected const float HANDLE_SIZE = 0.08f;
    protected const float PICK_SIZE = 0.10f;

    private Tool _lastTool;
    protected delegate void IndentedInspectorDelegate();
    private Dictionary<string, bool> _foldoutStatus = new Dictionary<string, bool>();

    void OnEnable() {
        _lastTool = Tools.current;
        Tools.current = Tool.None;
    }

    void OnDisable() {
        Tools.current = _lastTool;
    }


    protected void IndentedInspector(IndentedInspectorDelegate inspectorDelegate) {
        EditorGUI.indentLevel++;
        inspectorDelegate();
        EditorGUI.indentLevel--;
    }

    protected void IndentedInspector(string title, IndentedInspectorDelegate inspectorDelegate, bool openAtStart = true) {
        // TODO(Cristian): Have a map of the opened sections for we can correctly foldout
        bool opened = openAtStart;
        if (_foldoutStatus.ContainsKey(title)) {
            opened = _foldoutStatus[title];
        } else {
            _foldoutStatus.Add(title, opened);
        }

        opened = EditorGUILayout.Foldout(opened , title);
        if (opened) {
            EditorGUI.indentLevel++;
            inspectorDelegate();
            EditorGUI.indentLevel--;
        }
        _foldoutStatus[title] = opened;
    }

    protected void HorizontalBreak() {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

}
