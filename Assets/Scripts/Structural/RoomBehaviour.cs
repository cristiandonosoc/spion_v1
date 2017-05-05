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
    private BoxCollider _boxCollider;

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

    private void Init() {
        if (initialized) { return; }
        initialized = true;
        _data = new Data();
        _data.size = new Vector3(5f, 1f, 5f);
        name = "Room";
        // We put it to the parent
        var imb = SingletonBehaviour<InstanceManagerBehaviour>.Instance;
        transform.parent = imb.transform;
        Refresh();
    }

    public void Awake() {
        Init();
    }

    public void Refresh() {
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

    protected override void EditorAwake() {
        throw new NotImplementedException();
    }

    protected override void PlayModeAwake() {
        throw new NotImplementedException();
    }

    #region ACTIONS

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
