using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorBehaviour : CustomMonoBehaviour {
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

    #region MESSAGES

    [MessageKindMarker]
    public enum MessageKind {
        OPEN,
        CLOSE,
        CREATE_DIALOG,
        DESTROY_DIALOG
    }

    public override void ReceiveMessage(Type msgType, int msgValue, object payload = null) {
        if (msgType == typeof(MessageKind)) {
            ProcessMessage((MessageKind)msgValue, payload);
        }  else if (msgType == typeof(TriggerZoneMessage.MessageKind)) { 
            ProcessMessage((TriggerZoneMessage.MessageKind)msgValue, payload);
        } else {
            LogError("Received wrong MessageKind: {0}", msgType.FullName);
        }
    }

    private void ProcessMessage(MessageKind msg, object payload) {
        if (msg == MessageKind.OPEN) {
            Open();
        } else if (msg == MessageKind.CLOSE) {
            Close();
        }
    }

    private void ProcessMessage(TriggerZoneMessage.MessageKind msg, object payload) {
        var triggerPayload = (TriggerZoneMessage.MessagePayload)payload;

        Message internalMessage = triggerPayload.internalMessage;
        MessageKind internalMessageKind = (MessageKind)internalMessage.messageKind;

        if (msg == TriggerZoneMessage.MessageKind.ENTER) {
            if (internalMessageKind == MessageKind.CREATE_DIALOG) {
                EnterDoorZoneTrigger(triggerPayload.collider);
            }
        } else if (msg == TriggerZoneMessage.MessageKind.EXIT) {
            if (internalMessageKind == MessageKind.DESTROY_DIALOG) {
                ExitDoorZoneTrigger(triggerPayload.collider);
            }
        }
    }

    #endregion MESSAGES

    #region ACTIONS

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
        if (collider.tag != "Player") {
            return;
        }


        if (_dialogInstance) { return; }
        _dialogInstance = Instantiate<OpenBoxBehaviour>(OpenDialogPrefab);
        _dialogInstance.transform.parent = transform;
        _dialogInstance.transform.position = collider.transform.position + new Vector3(-2.5f, 2f, 1);
        _dialogInstance.fillness = 0;

        // We set the trigger
        _dialogInstance.player = collider.GetComponent<PlayerBehaviour>();
        if (_dialogInstance.player == null) {
            Log("NULL PLAYER");
        }
        _dialogInstance.door = this;
    }

    private void ExitDoorZoneTrigger(Collider test2) {
        if (_dialogInstance) {
            _dialogInstance.ReceiveMessage(OpenBoxBehaviour.MessageKind.CLOSE);
        }
        Close();
    }



    #endregion ACTIONS
}
