using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FloorBehaviour : CustomMonoBehaviour {

    #region UI

    public bool liveUpdatePrefabs;
    public bool snapVectors;
    public string panelHolderName = "Panels";
    public bool initialized = false;

    #endregion UI

    [Serializable]
    public class Data {
        public Vector3 size;

        public Data() {
            size = new Vector3();
        }
    }
    [SerializeField]
    private Data _data;

    [SerializeField]
    private BoxCollider _boxCollider;

    [SerializeField]
    private ModelBehaviour _panelModel;
    private ModelBehaviour[] _floorPanels;
    public ModelBehaviour PanelModel {
        get { return _panelModel; }
        set {
            _panelModel = value;
            _floorPanels = null;    // Recreate
        }
    }

    [SerializeField]
    private Vector3 _panelCount;
    public Vector3 PanelCount {
        get { return _panelCount; }
        set {
            _panelCount = value;
            _panelCount.y = 1;
            _floorPanels = null;    // Recreate
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

    public Vector3 Extents {
        get { return _data.size / 2; }
    }

    #endregion

    public void Awake() {
        Init();
    }

    private void Init() {
        if (initialized) { return; }
        _data = new Data();
        name = "Floor";
        initialized = true;
        Refresh();
    }

    public override void Refresh() {
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
        _boxCollider.size = _data.size;
    }

    #region ACTIONS

    public void SnapToRoom() {
        var room = transform.parent.GetComponent<RoomBehaviour>();
        var floorHeight = 0.2f;
        transform.localPosition = new Vector3(0, (-room.Size.y + floorHeight) / 2, 0);
        transform.localRotation = Quaternion.identity;
        _data.size = new Vector3(room.Size.x, floorHeight, room.Size.z);
        Refresh();
    }


    public void CalculatePanels(bool recreate) {
        if ((PanelModel == null) || (PanelCount.x <= 0) || (PanelCount.z <= 0)) {
            return;
        }

        if (recreate || (_floorPanels == null) || (_floorPanels.Length == 0)) {
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

            for (int z = 0; z < PanelCount.z; z++) {
                for (int x = 0; x < PanelCount.x; x++) {
                    ModelBehaviour panel = Instantiate<ModelBehaviour>(PanelModel);
                    panel.transform.parent = holderTransform;
                }
            }
            _floorPanels = GetComponentsInChildren<ModelBehaviour>();
        }


        Vector3 size = Size;
        Vector3 panelSize = VectorHelpers.Divide(size, PanelCount);
        Vector3 meshSize = PanelModel.GetComponent<MeshFilter>().sharedMesh.bounds.extents * 2;
        Vector3 scaleFactor = VectorHelpers.Divide(panelSize, meshSize);

        int i = 0;
        for (int z = 0; z < PanelCount.z; z++) {
            for (int x = 0; x < PanelCount.x; x++) {
                ModelBehaviour panel = _floorPanels[i];
                panel.transform.localRotation = panel.QuaternionFix;
                panel.transform.localScale = scaleFactor;
                Vector3 pos = -(Size / 2) + (panelSize / 2) + new Vector3(x * panelSize.x, 0, z * panelSize.z);
                panel.transform.localPosition = pos;
                i++;
            }
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
