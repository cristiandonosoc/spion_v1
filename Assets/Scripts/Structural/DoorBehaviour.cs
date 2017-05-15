using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorBehaviour : CustomMonoBehaviour {
    [MessageKindMarker]
    public enum MessageKind {
        OPEN,
        CLOSE,
        CREATE_DIALOG,
        DESTROY_DIALOG
    }

    private OpenBoxBehaviour _dialogInstance;

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

    public override void ReceiveMessage(Message msg) {
        if (msg.type == typeof(MessageKind)) {
            MessageKind messageKind = (MessageKind)msg.messageKind;
            if (messageKind == MessageKind.OPEN) {
                Open();
            } else if (messageKind == MessageKind.CLOSE) {
                Close();
            }
        } else if (msg.type == typeof(TriggerZoneMessage.MessageKind)) {
            var triggerKind = (TriggerZoneMessage.MessageKind)msg.messageKind;
            var triggerPayload = (TriggerZoneMessage.MessagePayload)msg.payload;

            Message internalMessage = triggerPayload.internalMessage;
            MessageKind internalMessageKind = (MessageKind)internalMessage.messageKind;

            if (triggerKind == TriggerZoneMessage.MessageKind.ENTER) {
                if (internalMessageKind == MessageKind.CREATE_DIALOG) {
                    EnterDoorZoneTrigger(triggerPayload.collider);
                }
            } else if (triggerKind == TriggerZoneMessage.MessageKind.EXIT) {
                if (internalMessageKind == MessageKind.DESTROY_DIALOG) {
                    ExitDoorZoneTrigger(triggerPayload.collider);
                }
            }
        } else {
            LogError("Wrong message type received: {0}", msg.type.ToString());
        }
    }

    private void Open() {
        if (DoorModelInstance.Animator.GetBool("Open")) {
            return;
        }
        DoorModelInstance.Animator.SetBool("Closed", false);
        DoorModelInstance.Animator.SetBool("Open", true);
        Collider.enabled = false;
    }

    private void Close() {
        if (DoorModelInstance.Animator.GetBool("Closed")) {
            return;
        }

        DoorModelInstance.Animator.SetBool("Open", false);
        DoorModelInstance.Animator.SetBool("Closed", true);
        Collider.enabled = true;
    }

    private void EnterDoorZoneTrigger(Collider collider) {
        if (_dialogInstance) { return; }
        _dialogInstance = Instantiate<OpenBoxBehaviour>(OpenDialogPrefab);
        _dialogInstance.transform.parent = transform;
        _dialogInstance.transform.position = collider.transform.position + new Vector3(-2.5f, 2f, 1);
        _dialogInstance.fillness = 0;

        // We set the trigger
        _dialogInstance.player = collider.GetComponent<PlayerBehaviour>();
        _dialogInstance.door = this;
    }

    private void ExitDoorZoneTrigger(Collider test2) {
        if (_dialogInstance) {
            _dialogInstance.ReceiveMessage(Message.Create(OpenBoxBehaviour.MessageKind.CLOSE));
        }
        Close();
    }



    #endregion ACTIONS
}
