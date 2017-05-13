using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : CustomMonoBehaviour {

    private OpenBoxBehaviour _dialogInstance;

    public void EnterDoorZoneTrigger(Collider collider) {
        if (_dialogInstance) { return; }
        _dialogInstance = Instantiate<OpenBoxBehaviour>(OpenDialogPrefab);
        _dialogInstance.transform.parent = transform;
        _dialogInstance.transform.position = collider.transform.position + new Vector3(-2.5f, 2f, 1);
        _dialogInstance.fillness = 0;

        // We set the trigger
        _dialogInstance.player = collider.GetComponent<PlayerBehaviour>();
        _dialogInstance.door = this;
    }

    public void ExitDoorZoneTrigger(Collider test2) {
        if (_dialogInstance) {
            _dialogInstance.EnterClosing();
        }
        Close();
    }

    #region SYNC DATA

    [Serializable]
    public class Data {
        public Vector3 size;

        public Data() {
            size = new Vector3(1, 1, 1);
        }
    }
    [SerializeField]
    private Data _data;

    #region GETTERS/SETTERS

    public Vector3 Size {
        get { return _data.size; }
        set { _data.size = value; }
    }

    /// <summary>
    /// Always half the size
    /// </summary>
    public Vector3 Extents {
        get { return _data.size / 2; }
    }

    #endregion GETTERS/SETTERS

    #endregion SYNC DATA

    #region UNITY DATA

    public bool snapVectors = true;

    [SerializeField]
    private BoxCollider _boxCollider;
    public BoxCollider Collider {
        get {
            if (_boxCollider == null) {
                _boxCollider = GetComponent<BoxCollider>();
                if (_boxCollider == null) {
                    _boxCollider = gameObject.AddComponent<BoxCollider>();
                }
            }
            return _boxCollider;
        }
    }

    [SerializeField]
    private AnimatedModelBehaviour _doorModel;
    private bool _recreateModel = true;
    public AnimatedModelBehaviour DoorModel {
        get { return _doorModel; }
        set {
            _doorModel = value;
            _recreateModel = true;
        }
    }

    private AnimatedModelBehaviour _doorModelInstance;
    public AnimatedModelBehaviour DoorModelInstance {
        get {
            if (_doorModelInstance == null) {
                _doorModelInstance = GetComponentInChildren<AnimatedModelBehaviour>();
            }
            return _doorModelInstance;
        }
    }

    [SerializeField]
    private OpenBoxBehaviour _openDialogPrefab;
    public OpenBoxBehaviour OpenDialogPrefab {
        get {
            if (_openDialogPrefab == null) {
                _openDialogPrefab = GetComponentInChildren<OpenBoxBehaviour>();
                if (_openDialogPrefab == null) {
                    _openDialogPrefab = new GameObject("OpenDialog").AddComponent<OpenBoxBehaviour>();
                    _openDialogPrefab.transform.parent = transform;
                }
            }
            return _openDialogPrefab;
        }
    }

    #endregion UNITY DATA

    #region LIFETIME MANAGEMENT

    protected override void EditorAwake() {
        if (_data == null) {
            _data = new Data();
        }
        Refresh();
    }

    protected override void PlayModeAwake() {
        Refresh();
    }

    public override void Refresh() {
        Collider.size = Size;

        if (_doorModel != null) {
            if (_recreateModel) {
                _doorModelInstance = RecreateModel();
                _recreateModel = false;
            }

            if (DoorModelInstance == null) {
                _doorModelInstance = RecreateModel();
            }

            Vector3 meshSize = DoorModelInstance.MeshSize;
            Vector3 scaleFactor = VectorHelpers.Divide(Size, meshSize);
            DoorModelInstance.transform.localScale = scaleFactor;

        }
    }

    private AnimatedModelBehaviour RecreateModel() {
        foreach (AnimatedModelBehaviour amb in GetComponentsInChildren<AnimatedModelBehaviour>()) {
            DestroyChildGameObject(amb.gameObject);
        }
        AnimatedModelBehaviour door = Instantiate<AnimatedModelBehaviour>(_doorModel);
        door.transform.parent = transform;
        door.transform.localPosition = Vector3.zero;
        return door;
    }

    #endregion LIFETIME MANAGEMENT

    #region ACTIONS

    public void Open() {
        if (DoorModelInstance.Animator.GetBool("Open")) {
            LogWarning("Trying to open already opened door");
            return;
        }
        DoorModelInstance.Animator.SetBool("Closed", false);
        DoorModelInstance.Animator.SetBool("Open", true);
        Collider.enabled = false;
    }

    public void Close() {
        if (DoorModelInstance.Animator.GetBool("Closed")) {
            LogWarning("Trying to close already closed door");
            return;
        }

        DoorModelInstance.Animator.SetBool("Open", false);
        DoorModelInstance.Animator.SetBool("Closed", true);
        Collider.enabled = true;
    }

    #endregion ACTIONS
}
