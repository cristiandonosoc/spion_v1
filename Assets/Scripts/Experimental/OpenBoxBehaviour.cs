using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenBoxBehaviour : CustomMonoBehaviour {

    public bool beingDestroyed = false;

    public DoorBehaviour door;
    public PlayerBehaviour player;

    [SerializeField]
    private Image _background;
    public Image Background {
        get {
            if (_background == null) {
                Image[] images = GetComponentsInChildren<Image>();
                foreach (Image image in images) {
                    if (image.name == "Background") {
                        _background = image;
                    }
                }
            }
            return _background;
        }

    }

    [SerializeField]
    private Image _bar;
    public Image Bar {
        get {
            if (_bar == null) {
                Image[] images = GetComponentsInChildren<Image>();
                foreach (Image image in images) {
                    if (image.name == "Bar") {
                        _bar = image;
                    }
                }
            }
            return _bar;
        }
    }

    public Color backgroundColor;
    public Color barColor;

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
    public float fillness = 1f;
    [Tooltip("Time to completely fill the bar (seconds)")]
    public float fillCompleteTime = 1f;
    [Tooltip("Time to completely unfill the bar (seconds)")]
    public float unfillCompleteTime = 1.75f;
    // TODO(Cristian): Found out why 1 works and what variable it should really be
    public float xScaleOffset = 1f;

    #region UPDATE

    public void Update() {
        if (beingDestroyed) { return; }
        UpdateBar();
        UpdatePlayerInput();
    }

    private void UpdateBar() {
        if (Background.color != backgroundColor) {
            Background.color = backgroundColor;
        }
        if (Bar.color != barColor) {
            Bar.color = barColor;
        }
        Bar.transform.localScale = Bar.transform.localScale.WithX(fillness);
        Bar.transform.localPosition = Bar.transform.localPosition.WithX(fillness - xScaleOffset);
    }

    private void UpdatePlayerInput() {
        if (player == null) { return; }
        if (Animator.GetBool("Active")) {
            if (Input.GetButton("Fire1")) {
                fillness += (1 / fillCompleteTime) * Time.deltaTime;
                if (fillness >= 1) {
                    EnterSuccess();
                }
            } else {
                fillness -= (1 / unfillCompleteTime) * Time.deltaTime;
                if (fillness <= 0) {
                    fillness = 0;
                }
            }
        }
    }

    #endregion UPDATE

    protected override void EditorAwake() {
        Refresh();
    }

    protected override void PlayModeAwake() {
        Refresh();
    }

    public override void Refresh() {
        Bar.transform.localScale = Bar.transform.localScale.WithX(fillness);
    }

    #region ANIMATION

    public void EnterActive() {
        Animator.SetBool("Opening", false);
        Animator.SetBool("Active", true);
    }

    public void EnterClosing() {
        Animator.SetBool("Active", false);
        Animator.SetBool("Closing", true);
    }

    public void EnterSuccess() {
        Animator.SetBool("Active", false);
        Animator.SetBool("Success", true);
        if (door) {
            door.Open();
        }
    }

    public void ExitClosing() {
        beingDestroyed = true;
        Destroy(gameObject);
    }

    #endregion ANIMATION

}
