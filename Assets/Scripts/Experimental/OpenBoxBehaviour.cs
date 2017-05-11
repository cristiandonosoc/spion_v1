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

    [SerializeField]
    private Animator _animator;
    public Animator Animator {
        get {
            if (_animator == null) {
                _animator = GetComponent<Animator>();
            }
            return _animator;
        }
    }

    [Range(0, 1)]
    public float scale = 1f;
    public float pressChange = 0.05f;
    public float noPressChange = 0.025f;
    // TODO(Cristian): Found out why 1 works and what variable it should really be
    public float xScaleOffset = 1f;

    public void Update() {
        Bar.transform.localScale = Bar.transform.localScale.WithX(scale);
        Bar.transform.localPosition = Bar.transform.localPosition.WithX(scale - xScaleOffset);
    }

    protected override void EditorAwake() {
        Refresh();
    }

    protected override void PlayModeAwake() {
        Refresh();
    }

    public override void Refresh() {
        Bar.transform.localScale = Bar.transform.localScale.WithX(scale);
    }

    public void Close() {
        Animator.SetBool("Close", true);
    }

    public void ByeBye() {
        Destroy(gameObject);
    }

}
