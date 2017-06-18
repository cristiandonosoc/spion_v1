using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EntityMonoBehaviour : CustomMonoBehaviour {

    [SerializeField]
    private uint _GUID = 0;
    public uint GUID {
        get { return _GUID; }
    }

    private TagBehaviour _tagBehaviour;

    public new void Awake() {
        if (_GUID == 0) {
            _GUID = SingletonBehaviour<GUIDManagerBehaviour>.Instance.GetGUID();
        }

        if (Application.isPlaying) {
            // We only want tags loaded on runtime?
            _tagBehaviour = GetComponent<TagBehaviour>();
        }
        // We pass back to the CustomMonoBehaviour 
        base.Awake();
    }

    /// <summary>
    /// Checks whether an object has all the tags associated
    /// </summary>
    /// <param name="tags">The tags to check</param>
    /// <returns>Whether the object has all the tags</returns>
    public bool HasTags(params Tag[] tags) {
        if (_tagBehaviour == null) {
            return false;
        }

        foreach (Tag tag in tags) {
            bool found = false;
            foreach (Tag ownedTag in _tagBehaviour.Tags) {
                if (tag == ownedTag) {
                    found = true;
                    break;
                }
            }
            if (!found) { return false; }
        }
        return true;
    }
}
