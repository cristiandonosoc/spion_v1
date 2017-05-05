using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle upon which the game will know that a model (mesh + anim + extras)
/// is being represented by the gameobject that contains it.
/// 
/// It also contains transformations needed to be done by the owner.
/// This was created mainly for walls. 
/// Could turn out to be a very bad pattern... Who knows? Adventure!
/// </summary>
public class ModelBehaviour : MonoBehaviour {

    public Vector3 rotationFix;

    public VectorHelpers.CoordChange ChangeXTo;
    public VectorHelpers.CoordChange ChangeYTo;
    public VectorHelpers.CoordChange ChangeZTo;

    public Quaternion QuaternionFix {
        get { return Quaternion.Euler(rotationFix); }
    }
}
