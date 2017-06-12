using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TriggerZoneMapping {
    public CustomMonoBehaviour target;
    public Message messageToSend;

    public TriggerZoneMapping(CustomMonoBehaviour target, Message messageToSend) {
        this.target = target;
        this.messageToSend = messageToSend;
    }
}

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class TriggerZoneBehaviour : CustomMonoBehaviour {

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
    public Data InternalData {
        get {
            if (_data == null) {
                _data = new Data();
            }
            return _data;
        }
    }

    #region GETTERS/SETTERS

    public Vector3 Size {
        get { return InternalData.size; }
        set { InternalData.size = value; }
    }

    /// <summary>
    /// Always half the size
    /// </summary>
    public Vector3 Extents {
        get { return InternalData.size / 2; }
    }

    #endregion GETTERS/SETTERS

    #endregion

    #region UNITY DATA

    public bool snapVectors = true;

    [SerializeField]
    private BoxCollider _collider;
    public BoxCollider Collider {
        get {
            if (_collider == null) {
                _collider = GetComponent<BoxCollider>();
                if (_collider == null) {
                    _collider = gameObject.AddComponent<BoxCollider>();
                }
            }
            return _collider;
        }
    }

    // TODO(Cristian): See how can we serialize this
    private List<TriggerZoneMapping> _enterReceivers;
    public List<TriggerZoneMapping> EnterReceivers {
        get {
            if (_enterReceivers == null) {
                _enterReceivers = new List<TriggerZoneMapping>();
            }
            return _enterReceivers;
        }
    }

    private List<TriggerZoneMapping> _exitReceivers;
    public List<TriggerZoneMapping> ExitReceivers {
        get {
            if (_exitReceivers == null) {
                _exitReceivers = new List<TriggerZoneMapping>();
            }
            return _exitReceivers;
        }
    }


    #endregion UNITY DATA


    protected override void EditorAwake() {
        Refresh();
    }

    protected override void PlayModeAwake() {
        Refresh();
    }

    public override void Refresh() {
        Collider.size = Size;
        Collider.isTrigger = true;
    }

    #region TRIGGER

    void OnTriggerEnter(Collider collider) {
        foreach (TriggerZoneMapping mapping in EnterReceivers) {
            // TODO(Cristian): Make this mapping occur in TriggerZoneManager, for better logging
            mapping.target.ReceiveMessage(mapping.messageToSend.type.type,
                                          mapping.messageToSend.messageKind,
                                          collider);
        }
    }

    void OnTriggerExit(Collider collider) {
        foreach (TriggerZoneMapping mapping in ExitReceivers) {
            // TODO(Cristian): Make this mapping occur in TriggerZoneManager, for better logging
            mapping.target.ReceiveMessage(mapping.messageToSend.type.type,
                                          mapping.messageToSend.messageKind,
                                          collider);
        }
    }


    #endregion TRIGGER


}
