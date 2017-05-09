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

    public bool drawGizmos = false;
    public Vector3 rotationFix;

    public VectorHelpers.CoordChange ChangeXTo;
    public VectorHelpers.CoordChange ChangeYTo;
    public VectorHelpers.CoordChange ChangeZTo;

    public Quaternion QuaternionFix {
        get { return Quaternion.Euler(rotationFix); }
    }

    void OnDrawGizmos() {
        if (!drawGizmos) { return; }
        Color oldColor = Gizmos.color;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, MeshSize);
        Gizmos.color = oldColor;
    }

    public bool refreshMeshSize = true;

    private Vector3 _meshSize;
    public Vector3 MeshSize {
        get {
            if (!refreshMeshSize && _meshSize != Vector3.zero) {
                return _meshSize;
            }
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

            Bounds bounds = new Bounds();
            foreach (MeshFilter mf in meshFilterList) {
                Bounds meshBound = mf.sharedMesh.bounds;
                meshBound.center = mf.transform.localPosition;
                Debug.Log(meshBound);
                bounds.Encapsulate(meshBound);
            }


            refreshMeshSize = false;
            _meshSize = bounds.size;
            return _meshSize;
        }
    }


}
