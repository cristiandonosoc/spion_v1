using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class RoomBehaviour : CustomMonoBehaviour {

    #region UI

    public bool snapVectors;
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
            Refresh();
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
        BlockBehaviour block = new GameObject().AddComponent<BlockBehaviour>();
        block.transform.parent = transform;

        var imb = SingletonBehaviour<InstanceManagerBehaviour>.Instance;
        block.BlockModel = imb.defaultBlockModel;
        block.Refresh();
        return block;
    }

    //public FloorBehaviour AddFloor() {
    //    FloorBehaviour floor = new GameObject().AddComponent<FloorBehaviour>();
    //    floor.transform.parent = transform;
    //    floor.PanelModel = defaultFloorPanel;
    //    floor.PanelCount = defaultFloorPanelCount;
    //    floor.SnapToRoom();
    //    floor.CalculatePanels(recreate: true);
    //    return floor;
    //}

    //public WallBehaviour AddWall(int wallIndex = -1) {
    //    var wall = new GameObject().AddComponent<WallBehaviour>();
    //    wall.transform.parent = transform;
    //    wall.WallModel = defaultWall;
    //    wall.WallPanelCount = defaultWallCount;
    //    wall.SnapToRoom(wallIndex);
    //    return wall;
    //}

    //public DoorBehaviour AddDoor() {

    //    DoorBehaviour door = new GameObject().AddComponent<DoorBehaviour>();
    //    door.transform.parent = transform;
    //    return door;
    //}

    #endregion ACTIONS
}
