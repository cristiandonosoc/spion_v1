using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    public TriggerZoneDelegate EnterDelegate;
    public TriggerZoneDelegate ExitDelegate;


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
        if (EnterDelegate != null) {
            EnterDelegate(collider);
        }
    }

    void OnTriggerExit(Collider collider) {
        if (ExitDelegate != null) {
            ExitDelegate(collider);
        }
    }


    #endregion TRIGGER


}
