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
            size = new Vector3(1, 1, 1);
        }
    }
    [SerializeField]
    private Data _data;

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

    #endregion SYNC DATA

    #region UNITY DATA

    public bool snapVectors = true;

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

    [SerializeField]
    private ModelBehaviour _blockModel;
    /// <summary>
    /// Whether to recreate the model next Refresh call
    /// </summary>
    private bool _recreateModel = true;
    private ModelBehaviour _blockModelInstance;
    public ModelBehaviour BlockModel {
        get { return _blockModel; }
        set {
            _blockModel = value;
            _recreateModel = true;
        }
    }

    #endregion



    #region LIFETIME MANAGEMENT

    protected override void EditorAwake() {
        _data = new Data();
        Refresh();
    }

    protected override void PlayModeAwake() {
        Refresh();
    }

    public override void Refresh() {
        BoxCollider.size = Size;

        if (_blockModel) {
            // We see if we're talking about the same model
            if (_recreateModel) {
                _blockModelInstance = RecreateModel();
                _recreateModel = false;
            }

            if (_blockModelInstance == null) {
                _blockModelInstance = GetComponentInChildren<ModelBehaviour>();
                if (_blockModelInstance == null) {
                    _blockModelInstance = RecreateModel();
                }
            }

            Vector3 meshSize = _blockModel.MeshSize;
            Vector3 scaleFactor = VectorHelpers.Divide(Size, meshSize);
            _blockModelInstance.transform.localScale = scaleFactor;
        }
    }

    private ModelBehaviour RecreateModel() {
        foreach (ModelBehaviour mb in GetComponentsInChildren<ModelBehaviour>()) {
            DestroyChildGameObject(mb.gameObject);
        }
        ModelBehaviour block = Instantiate<ModelBehaviour>(_blockModel);
        block.transform.parent = transform;
        block.transform.localPosition = Vector3.zero;
        return block;
    }

    #endregion LIFETIME MANAGEMENT
}
