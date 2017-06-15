using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TagBehaviour))]
public class TagBehaviourEditor : SpecializedInspector {

    private TagBehaviour _target;
    private Tag _selectedTag;

    public override void OnInspectorGUI() {
        _target = (TagBehaviour)target;

        IndentedInspector("Tags", TagInspector);
        HorizontalBreak();
        IndentedInspector("Debug", DebugInspector, false);
    }

    private void TagInspector() {
        Tag newTag = Tag.PLAYER;        // Ignore initial value
        Tag changeTag = Tag.PLAYER;     // Ignore initial value
        int changeIndex = -1;
        int removeIndex = -1;
        for (int i = 0; i < _target.tags.Count; i++) {
            EditorGUILayout.BeginHorizontal();

            Tag tag = _target.tags[i];
            newTag = (Tag)EditorGUILayout.EnumPopup(tag);
            if (tag != newTag) {
                changeIndex = i;
                changeTag = newTag;
            }
            if (GUILayout.Button("Remove")) {
                removeIndex = i;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (removeIndex != -1) {
            _target.tags.RemoveAt(removeIndex);
            _target.tags.Sort();

        } else if (changeIndex != -1) {
            if (!_target.tags.Contains(changeTag)) {
                _target.tags[changeIndex] = newTag;
                _target.tags.Sort();
            } else {
                RepeatedWarning(changeTag);
            }
        }

        HorizontalBreak();

        EditorGUILayout.BeginHorizontal();

        _selectedTag = (Tag)EditorGUILayout.EnumPopup(_selectedTag);
        if (GUILayout.Button("Add")) {
            if (!_target.tags.Contains(_selectedTag)) {
                _target.tags.Add(_selectedTag);
                _target.tags.Sort();
            } else {
                RepeatedWarning(_selectedTag);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void RepeatedWarning(Tag tag) {
        Debug.LogWarningFormat("GameObject \"{0}\" already has tag \"{1}\"",
                               _target.name, tag);
    }

    private void DebugInspector() {
        DrawDefaultInspector();
    }
}
