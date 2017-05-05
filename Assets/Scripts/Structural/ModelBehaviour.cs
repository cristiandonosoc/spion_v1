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

    // Don't serialize this
    private Vector3 _meshSize;

    public Vector3 MeshSize {
        get {
            // TODO(Cristian): Maybe cache this? This shouldn't be called in runtime more than once anyway...
            List<MeshFilter> meshFilterList = new List<MeshFilter>();
            MeshFilter selfMeshFilter = GetComponent<MeshFilter>();
            if (selfMeshFilter) {
                meshFilterList.Add(selfMeshFilter);
            }

            foreach(MeshFilter mf in GetComponentsInChildren<MeshFilter>()) {
                meshFilterList.Add(mf);
            }

            if (meshFilterList.Count == 0) {
                throw new System.Exception("No Mesh Filter");
            }

            float maxSqrMagnitude = 0;
            Vector3 meshSize = Vector3.zero;
            foreach(MeshFilter mf in meshFilterList) {
                Vector3 size = mf.sharedMesh.bounds.size;
                float currentSqrMagnitude = size.sqrMagnitude;
                if (currentSqrMagnitude > maxSqrMagnitude) {
                    currentSqrMagnitude = maxSqrMagnitude;
                    meshSize = size;
                }
            }
            return meshSize;
        }
    }


}
