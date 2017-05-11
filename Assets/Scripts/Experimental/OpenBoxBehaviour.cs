using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenBoxBehaviour : CustomMonoBehaviour {

    public DoorBehaviour door;
    public PlayerBehaviour player;

    [SerializeField]
    private Image _bar;
    public Image Bar {
        get {
            if (_bar == null) {
                _bar = GetComponentInChildren<Image>();
            }
            return _bar;
        }
    }

    public override void Refresh() {
    }

    protected override void EditorAwake() {
        Refresh();
    }

    protected override void PlayModeAwake() {
        Refresh();
    }
}
