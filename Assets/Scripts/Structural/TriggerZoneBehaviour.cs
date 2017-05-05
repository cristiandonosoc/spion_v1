using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TriggerZoneBehaviour : MonoBehaviour {

    public delegate void VoidCallback();

    #region SYNC DATA

    public bool initialized = false;

    [Serializable]
    public class Data {
        public Vector3 size;

        public Data() {
            size = new Vector3();
        }
    }
    [SerializeField]
    private Data _data;

    private BoxCollider _boxCollider;

    #endregion SYNC DATA


    public void Awake() {
        Init();
        ReReference();
    }

    public void Init() {
        if (initialized) { return; }

    }

    public void ReReference() {

    }


}
