using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DoorBehaviour : CustomMonoBehaviour {

    #region UI

    public bool open;
    private Animator _animator;
    public bool initialized = false;

    #endregion UI

    #region SYNC DATA

    [Serializable]
    public class Data {
        public Vector3 size;

        public Data() {
            size = new Vector3();
        }
    }
    [SerializeField]
    private Data _data;

    public Vector3 Size {
        get { return _data.size; }
        set {
            _data.size = value;
            Refresh();
        }

    }

    #endregion SYNC DATA

    [SerializeField]
    private BoxCollider _boxCollider;

    [SerializeField]
    private ModelBehaviour _doorModel;
    private ModelBehaviour _doorModelInstance;
    public ModelBehaviour DoorModel {
        get { return _doorModel; }
        set {
            _doorModel = value;
        }
    }

    public void Awake() {
        Init();
        ReReference();
    }

    private void Init() {
        if (initialized) { return; }
        _data = new Data();
        name = "Door";
        initialized = true;
        Refresh();
    }

    /// <summary>
    /// Tries to obtain all references needed in order to function.
    /// Might return null when called from the editor.
    /// Have to call this method because Unity reference system is bullshit
    /// and don't persist on the EDITOR -> GAME boundary
    /// </summary>
    private void ReReference() {
        _boxCollider = GetComponent<BoxCollider>();
        _doorModelInstance = GetComponentInChildren<ModelBehaviour>();
        _animator = _doorModelInstance.GetComponent<Animator>();
    }

    public override void Refresh() {
        RecreateBoundingBox();
    }

    private void RecreateBoundingBox() {
        if (_boxCollider == null) {
            _boxCollider = GetComponent<BoxCollider>();
            if (_boxCollider == null) {
                _boxCollider = gameObject.AddComponent<BoxCollider>();
            }
        }

        _boxCollider.center = Vector3.zero;
        _boxCollider.size = Size;
    }

    #region ACTIONS

    public void CalculatePrefabs(bool recreate) {
        if ((DoorModel == null)) {
            return;
        }

        if (recreate) {
            ModelBehaviour[] models = GetComponentsInChildren<ModelBehaviour>();
            foreach (ModelBehaviour model in models) {
#if UNITY_EDITOR
                DestroyImmediate(model.gameObject);
#endif
            }
        }

        _doorModelInstance = Instantiate<ModelBehaviour>(DoorModel);
        _doorModelInstance.transform.parent = transform;
        _doorModelInstance.transform.localPosition = Vector3.zero;
        _doorModelInstance.transform.localRotation = Quaternion.Euler(_doorModelInstance.rotationFix);

        _animator = _doorModelInstance.GetComponent<Animator>();
        SizeToRoom();
    }

    public void SizeToRoom() {
        RoomBehaviour room = transform.parent.GetComponent<RoomBehaviour>();


        // We find the size of the door
        MeshFilter[] meshRenderers = GetComponentsInChildren<MeshFilter>();
        float maxSqrMagnitude = 0;
        Vector3 meshSize = Vector3.zero;
        foreach(MeshFilter meshRenderer in meshRenderers) {
            Vector3 size = meshRenderer.sharedMesh.bounds.size;
            float currentSqrMagnitude = size.sqrMagnitude;
            if (currentSqrMagnitude > maxSqrMagnitude) {
                currentSqrMagnitude = maxSqrMagnitude;
                meshSize = size;
            }
        }

        float scaleFactor = room.Size.y / meshSize.y;
        Vector3 realSize = meshSize * scaleFactor;

        // We get the collider
        Size = realSize;

        _doorModelInstance.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    public void Open() {
        if (!open) {
            open = true;
            _animator.SetTrigger("OpenTrigger");
        }
    }

    public void Close() {
        if (open) {
            open = false;
            _animator.SetTrigger("CloseTrigger");
        }
    }

    protected override void EditorAwake() {
        throw new NotImplementedException();
    }

    protected override void PlayModeAwake() {
        throw new NotImplementedException();
    }

    #endregion ACTIONS



}
