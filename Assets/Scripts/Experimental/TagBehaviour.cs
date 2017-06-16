using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Tag {
    PLAYER,
    ENEMY,
}

public class TagBehaviour : MonoBehaviour {
    [SerializeField]
    public List<Tag> _tags;
    public List<Tag> Tags {
        get {
            if (_tags == null) {
                _tags = new List<Tag>();
            }
            return _tags;
        }
    }
}
