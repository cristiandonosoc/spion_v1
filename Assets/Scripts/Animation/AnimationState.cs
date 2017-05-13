using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Attribute to be put to every enum that is to be detectable as an AnimationState machine.
/// This will enable the editor to detect them and to "easily" put the correct state into the machine.
/// </summary>
public class AnimationEnum : Attribute { }

/// <summary>
/// Represents all the kind of events that can be triggered by an animation.
/// The idea is that the event, plus the internal state of the Animator and
/// the holder object *should* be sufficient to do all kinds of effects.
/// 
/// Very much a test.
/// </summary>
public enum AnimationStateEvent {
    ANIMATION_START,
    ANIMATION_END
}

public class AnimationState : StateMachineBehaviour {

    public Type enumType;
    public int enumValue;

    private CustomMonoBehaviour _target;
    private CustomMonoBehaviour GetTarget(Animator animator) {
        if (_target == null) {
            _target = animator.GetComponent<CustomMonoBehaviour>();
        }
        return _target;
    }

    // NOTE(Cristian): For now we only care for Enter and Exit

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // We update the state variable (So this is queryable by the Animator users)
        animator.SetInteger("State", enumValue);

        CustomMonoBehaviour target = GetTarget(animator);
        target.AnimationStateChange(AnimationStateEvent.ANIMATION_START, enumValue);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        CustomMonoBehaviour target = GetTarget(animator);
        target.AnimationStateChange(AnimationStateEvent.ANIMATION_END, enumValue);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

}