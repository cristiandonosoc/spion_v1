using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwordBehaviour : CustomMonoBehaviour {

    [AnimationEnum]
    public enum AnimationStates {
        IDLE,
        ATTACKING 
    }

    private AnimationStates _currentAnimationState = AnimationStates.IDLE;

    [MessageKindMarker]
    public enum MessageKind {
        ATTACK
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

    public override void ReceiveMessage(Message msg) {
        if (msg.type == typeof(MessageKind)) {
            MessageKind messageKind = (MessageKind)msg.messageKind;
            if (messageKind == MessageKind.ATTACK) {
                if (_currentAnimationState == AnimationStates.IDLE) {
                    Animator.SetTrigger("EnterAttack");
                }
            }
        } else {
            LogError("Received wrong MessageKind: {0}", msg.type.ToString());
        }
    }

    public override bool GetAnimationStateEvents() {
        return true;
    }

    public override void AnimationStateChange(AnimationStateEvent animationEvent, int stateValue) {
        // We cast the value
        AnimationStates state = (AnimationStates)stateValue;
        if ((animationEvent == AnimationStateEvent.ANIMATION_ENTER) &&
            (_currentAnimationState == state) &&
            (state == AnimationStates.IDLE)) {
            return;
        }

        if (animationEvent == AnimationStateEvent.ANIMATION_ENTER) {
            _currentAnimationState = state;
            if (_currentAnimationState == AnimationStates.ATTACKING) {
                TrailRenderer.enabled = true;
            } else if (_currentAnimationState == AnimationStates.IDLE) {
                TrailRenderer.enabled = false;
                if (Parent != null) {

                    Parent.ReceiveMessage(Message.Create(PlayerBehaviour.MessageKind.ATTACK_STOPPED));
                }
            }
        }
    }

    private TrailRenderer _trailRenderer;
    public TrailRenderer TrailRenderer {
        get {
            if (_trailRenderer == null) {
                _trailRenderer = GetComponent<TrailRenderer>();
            }
            return _trailRenderer;
        }
    }

    public PlayerBehaviour Parent;

    protected override void PlayModeAwake() {
        _currentAnimationState = AnimationStates.IDLE;
        TrailRenderer.enabled = false;
    }

    void OnTriggerEnter(Collider other) {
        EnemyBehaviour enemy = other.GetComponent<EnemyBehaviour>();
        if (enemy == null) { return; }
        if (_currentAnimationState == AnimationStates.IDLE) { return; }

        //enemy.ReceiveMessage(Message.Create(EnemyBehaviour.MessageKind.HIT));
        enemy.ReceiveMessage(EnemyBehaviour.MessageKind.HIT);
    }
}
