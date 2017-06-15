using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Tag {
    PLAYER,
    ENEMY,
}

public class TagBehaviour : MonoBehaviour {
    public List<Tag> tags;
}
