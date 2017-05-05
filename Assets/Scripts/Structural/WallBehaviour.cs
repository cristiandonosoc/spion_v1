using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WallBehaviour : CustomMonoBehaviour {

    #region UI

    public bool snapVectors;
    public bool liveUpdatePrefabs;
    public string panelHolderName = "Panels";
    public bool initialized = false;
    public float wallWidth = 0.2f;

    #endregion

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

    [SerializeField]
    private BoxCollider _boxCollider;

    [SerializeField]
    private ModelBehaviour _wallModel;
    private ModelBehaviour[] _wallPanels;
    public ModelBehaviour WallModel {
        get { return _wallModel; }
        set {
            _wallModel = value;
            _wallPanels = null;     // Recreatea
        }
    }

    [SerializeField]
    private int _wallCount;
    public int WallCount {
        get { return _wallCount; }
        set {
            _wallCount = value;
            _wallPanels = null;     // Recreate
        }
    }

    private Vector3 _wallPanelCount;
    public Vector3 WallPanelCount {
        get { return _wallPanelCount; }
        set {
            _wallPanelCount = value;
            _wallPanels = null;     // Recreate
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

    #endregion GETTERS/SETTERS

    private void Init() {
        if (initialized) { return; }
        _data = new Data();
        initialized = true;
        liveUpdatePrefabs = true;
        name = "Wall";
        Refresh();
    }

    public void Awake() {
        Init();
    }

    public override void Refresh() {
        RecreateBoundingBox();
        if (liveUpdatePrefabs) {
            CalculatePrefabs(recreate: false);
        }
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

    #region ACTIONS

    public void SnapToRoom(int wallIndex) {
        // TODO(Cristian): Should snapping consider floor (for initial height)?
        // TODO(Cristian): Should walls be outside the room (minor 0.1f)?
        var room = transform.parent.GetComponent<RoomBehaviour>();

        if (wallIndex == 0) {   // NORTH
            transform.localPosition = new Vector3(0, 0, room.Size.z / 2);
            transform.rotation = Quaternion.Euler(0, 180, 0);
            _data.size = new Vector3(room.Size.x, room.Size.y, wallWidth);
        } else if (wallIndex == 1) {    // EAST
            transform.localPosition = new Vector3(room.Size.x / 2, 0, 0);
            transform.rotation = Quaternion.Euler(0, 270, 0);
            _data.size = new Vector3(room.Size.z, room.Size.y, wallWidth);
        } else if (wallIndex == 2) {    // SOUTH
            transform.localPosition = new Vector3(0, 0, -room.Size.z / 2);
            transform.rotation = Quaternion.identity;
            _data.size = new Vector3(room.Size.x, room.Size.y, wallWidth);
        } else if (wallIndex == 3) {    // WEST
            transform.localPosition = new Vector3(-room.Size.x / 2, 0, 0);
            transform.rotation = Quaternion.Euler(0, 90, 0);
            _data.size = new Vector3(room.Size.z, room.Size.y, wallWidth);
        } else {
            return;
        }

        if (wallIndex > -1) {
            if ((wallIndex % 2) == 0) {     // Pair means SOUTH-NORTH
                WallCount = (int)WallPanelCount.x;
            } else {
                WallCount = (int)WallPanelCount.z;
            }
        }

        Refresh();
    }

    public void CalculatePrefabs(bool recreate) {
        if ((WallModel == null) || (WallCount <= 0)) {
            return;
        }
        if (recreate || (_wallPanels == null) || (_wallPanels.Length == 0)) {
            Transform holderTransform = transform.FindChild(panelHolderName);
            if (holderTransform != null) {
#if UNITY_EDITOR
                DestroyImmediate(holderTransform.gameObject);
#else
                Destroy(holderTransform.gameObject);
#endif
            }
            holderTransform = new GameObject(panelHolderName).transform;
            holderTransform.parent = transform;
            holderTransform.localPosition = Vector3.zero;
            holderTransform.localRotation = Quaternion.identity;

            for (int i = 0; i < WallCount; i++) {
                ModelBehaviour wallPanel = Instantiate<ModelBehaviour>(WallModel);
                wallPanel.transform.parent = holderTransform;
            }
            _wallPanels = GetComponentsInChildren<ModelBehaviour>();
        }

        Vector3 panelSize = new Vector3(_data.size.x / WallCount, _data.size.y, _data.size.z);
        Vector3 originalMeshSize = WallModel.GetComponent<MeshFilter>().sharedMesh.bounds.extents * 2;
        var meshSize = new Vector3(originalMeshSize.z, originalMeshSize.y, originalMeshSize.x);
        Vector3 scaleFactor = VectorHelpers.Divide(panelSize, meshSize);

        for (int i = 0; i < WallCount; i++) {
            ModelBehaviour panel = _wallPanels[i];
            Vector3 pos = -Extents + (panelSize / 2) + new Vector3(i * panelSize.x, 0, 0);
            panel.transform.localPosition = pos;
            panel.transform.localRotation = Quaternion.Euler(panel.rotationFix);
            panel.transform.localScale = VectorHelpers.ChangeCoords(scaleFactor, panel.ChangeXTo, panel.ChangeYTo, panel.ChangeZTo);
            //var collider = panel.gameObject.AddComponent<BoxCollider>();
            //collider.size = originalMeshSize;
        }
    }

    protected override void EditorAwake() {
        throw new NotImplementedException();
    }

    protected override void PlayModeAwake() {
        throw new NotImplementedException();
    }

    #endregion ACTIONS
}
