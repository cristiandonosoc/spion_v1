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
    ANIMATION_ENTER,
    ANIMATION_EXIT
}

/// <summary>
/// This class is to be added to all the relevant states of an Animator attached to a CustomMonobehaviour.
/// This class permits an easy way to get callbacks when the animator enter/leaves a tracked state,
/// enabling better code for coordinating with animations.
/// 
/// The CustomMonoBehaviour that will receive the callbacks has to define an enum that represents
/// the states to which he is interested on receiving callbacks. He must mark the enum as [AnimationEnum].
/// This permits the editor to use reflection and add good dropdowns to enable this information.
/// Later the object will receive an AnimationStateEvent with two parameters: what event happened (enter/exit)
/// and to what state it happened.
///
/// The first example of using this is OpenBoxBehaviour.
/// </summary>
public class AnimationState : UnityEngine.StateMachineBehaviour {

    public SerializableType enumType;
    public int enumValue;

    private List<CustomMonoBehaviour> _targets;
    private List<CustomMonoBehaviour> GetTargets(Animator animator) {
        if (_targets == null) {
            CustomMonoBehaviour[] potentialTargets = animator.GetComponents<CustomMonoBehaviour>();
            _targets = new List<CustomMonoBehaviour>();
            foreach (CustomMonoBehaviour potentialTarget in potentialTargets) {
                if (potentialTarget.GetAnimationStateEvents()) {
                    _targets.Add(potentialTarget);
                }
            }
        }
        return _targets;
    }

    // NOTE(Cristian): For now we only care for Enter and Exit. Keeping the comments for future reference
    //                 of what kind of events we might want to trigger.

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // We update the state variable (So this is queryable by the Animator users)
        animator.SetInteger("State", enumValue);

        List<CustomMonoBehaviour> targets = GetTargets(animator);
        foreach (CustomMonoBehaviour target in targets) {
            target.AnimationStateChange(AnimationStateEvent.ANIMATION_ENTER, enumValue);
        }

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        List<CustomMonoBehaviour> targets = GetTargets(animator);
        foreach (CustomMonoBehaviour target in targets) {
            target.AnimationStateChange(AnimationStateEvent.ANIMATION_EXIT, enumValue);
        }
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