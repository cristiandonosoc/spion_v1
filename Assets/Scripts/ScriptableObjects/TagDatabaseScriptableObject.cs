using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tag : IComparable<Tag> {
    public int id;
    public string name;

    public int CompareTo(Tag other) {
        return id - other.id;
    }
}

[CreateAssetMenu(fileName = "SO_TagDatabase", menuName = "Scriptable Objects/Tag Database")]
public class TagDatabaseScriptableObject : ScriptableObject {

    #region STATIC

    private static TagDatabaseScriptableObject _tagDatabase;
    public static TagDatabaseScriptableObject Instance {
        get {
            if (_tagDatabase == null) {
                var tagDbs = Resources.FindObjectsOfTypeAll<TagDatabaseScriptableObject>();
                if (tagDbs.Length != 1) {
                    throw new InvalidProgramException(string.Format("Wrong number of Tag Databases: {0}", 
                                                                    tagDbs.Length));
                }
                _tagDatabase = tagDbs[0];
            }
            return _tagDatabase;
        }
    }

    /// <summary>
    /// This is so the static Instance method is forced to reload.
    /// Should only be called by the inspector
    /// </summary>
    public static void ClearInstance() {
        _tagDatabase = null;
    }

    #endregion

    public List<Tag> tags;

    public Tag Find(int id) {
        foreach (Tag tag in tags) {
            if (tag.id == id) {
                return tag;
            }
        }
        return null;
    }

    public void RemoveTags(Tag[] removeTags) {
        foreach (Tag tag in removeTags) {
            tags.Remove(tag);
        }
        Sort();
    }

    public bool AddTags(Tag tag) {
        if (tags.BinarySearch(tag) != -1) {
            return false;
        }
        tags.Add(tag);
        Sort();
        return true;
    }

    public void Sort() {
        tags.Sort();
    }

}