using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlockBehaviour : CustomMonoBehaviour {

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

    #endregion SYNC DATA

    #region UNITY DATA

    [SerializeField]
    private BoxCollider _boxCollider;
    public BoxCollider BoxCollider {
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

    #endregion



    #region LIFETIME MANAGEMENT

    protected override void EditorAwake() {
        _data = new Data();
        Log("Editor");
    }

    protected override void PlayModeAwake() {
        Log("PlayMode");
    }

    #endregion LIFETIME MANAGEMENT



}
