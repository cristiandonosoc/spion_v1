using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class RoomBehaviour : CustomMonoBehaviour {

    #region UI

    public bool snapVectors = true;
    public bool initialized = false;

    public ModelBehaviour defaultFloorPanel;
    public Vector3 defaultFloorPanelCount;
    public ModelBehaviour defaultWall;
    public Vector3 defaultWallCount;

    #endregion

    [Serializable]
    public class Data {
        public Vector3 size;

        public Data() {
            size = new Vector3();
        }
    }

    [SerializeField]
    private Data _data;
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

    #region GETTERS/SETTERS

    public Vector3 Size {
        get { return _data.size; }
        set {
            _data.size = value;
        }
    }

    /// <summary>
    /// Always half the size
    /// </summary>
    public Vector3 Extents {
        get { return _data.size / 2; }
    }

    #endregion

    public override void Refresh() {
        Collider.size = Size;
    }

    protected override void EditorAwake() {
        _data = new Data();
        _data.size = new Vector3(5f, 1f, 5f);
        name = "Room";
        // We put it to the parent
        var imb = SingletonBehaviour<InstanceManagerBehaviour>.Instance;
        transform.parent = imb.transform;
        Collider.isTrigger = true;
        Refresh();
    }

    protected override void PlayModeAwake() {
        Refresh();
    }

    #region ACTIONS

    public BlockBehaviour AddBlock() {
        // We count hoy many blocks we have
        int blockCount = GetComponentsInChildren<BlockBehaviour>().Length;
        BlockBehaviour block = new GameObject().AddComponent<BlockBehaviour>();
        block.name = "Block_" + blockCount;
        block.transform.parent = transform;
        block.transform.localPosition = Vector3.zero;

        // We get the default block model
        var imb = SingletonBehaviour<InstanceManagerBehaviour>.Instance;
        block.BlockModel = imb.defaultBlockModel;
        block.Refresh();
        return block;
    }

    public DoorBehaviour AddDoor() {
        // We cound hoy many doors we have
        int doorCount = GetComponentsInChildren<DoorBehaviour>().Length;
        DoorBehaviour door = new GameObject().AddComponent<DoorBehaviour>();
        door.name = "Door_" + doorCount;
        door.transform.parent = transform;
        door.transform.localPosition = Vector3.zero;

        // We get the default door model
        var imb = SingletonBehaviour<InstanceManagerBehaviour>.Instance;
        door.DoorModel = imb.defaultDoorModel;
        door.Refresh();
        return door;
    }

    #endregion ACTIONS
}
