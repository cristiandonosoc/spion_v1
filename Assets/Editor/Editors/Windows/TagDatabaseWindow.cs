using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TagDatabaseWindow : EditorWindow {

    [MenuItem("Window/Tag Database")]
    public static void ShowWindow() {
        EditorWindow.GetWindow<TagDatabaseWindow>();
    }

    TagDatabaseScriptableObject _tagDatabase;
    Tag _newTag;

    void OnEnable() {
        _tagDatabase = TagDatabaseScriptableObject.Instance;
    }

    void OnDisable() {
        if (_tagDatabase != null) {
            _tagDatabase.Sort();
            EditorUtility.SetDirty(_tagDatabase);
            AssetDatabase.SaveAssets();
            TagDatabaseScriptableObject.ClearInstance();
        }
    }

    void OnGUI() {

        EditorGUILayout.HelpBox("If you change the id number and will be resorted, " +
                                "the number will appear repeated somewhere strange. " +
                                "That's a unity glitch. Click somewhere else and it will update correctly.",
                                MessageType.Info);

        List<Tag> removeList = new List<Tag>();
        bool sort = false;
        foreach (Tag tag in _tagDatabase.tags) {
            EditorGUILayout.BeginHorizontal();
            int id = EditorGUILayout.IntField(tag.id);
            if (id != tag.id) {
                tag.id = id;
                sort = true;
            }


            tag.name = EditorGUILayout.TextField(tag.name);
            if (GUILayout.Button("Remove")) {
                removeList.Add(tag);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (removeList.Count > 0) {
            _tagDatabase.RemoveTags(removeList.ToArray());
        } else if (sort) {
            _tagDatabase.Sort();
        }

        // Horizontal break
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.BeginHorizontal();
        if (_newTag == null) { _newTag = new Tag(); }
        _newTag.id = EditorGUILayout.IntField(_newTag.id);
        _newTag.name = EditorGUILayout.TextField(_newTag.name);
        if (GUILayout.Button("Add")) {
            if (_newTag.name == "") {
                Debug.LogError("Empty name not valid");
            } else {
                if (!_tagDatabase.AddTags(_newTag)) {
                    Tag collisionTag = _tagDatabase.Find(_newTag.id);
                    Debug.LogErrorFormat("Tag with the same id ({0}) exists: {1}", collisionTag.id, collisionTag.name);
                } else {
                    // We free the instance so we don't modify the one in the database
                    _newTag = null;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
