using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


/// <summary>
/// Base class for important elements in within the game.
/// Mainly CustomMonoBehaviour gives:
/// 1. Editor/PlayMode Awake methods. This is mainly used for editor code that is run on editor runtime.
/// 2. ReceiveMessage: A generic method to receive Messages. This is the official way of loosely-coupled
///    communication between objects. 
///    See Message for more information.
/// 3. AnimationStateChange. This is a callback called by AnimationState to owning monobehaviours of an Animator.
///    This is useful for having better control on when a state changes.
///    See AnimationState and AnimationStateEvent for more information.
/// </summary>
[ExecuteInEditMode]
public abstract class CustomMonoBehaviour : MonoBehaviour {

    private string FormatMessage(string message, params object[] args) {
        return string.Format("[Game Object: \"{0}\", Component: \"{1}\"] => {2}",
                             gameObject.name,
                             GetType(),
                             string.Format(message, args));
    }

    public void Log(string message, params object[] args) {
        Debug.Log(FormatMessage(message, args));
    }

    public void LogWarning(string message, params object[] args) {
        Debug.LogWarning(FormatMessage(message, args));
    }

    public void LogError(string message, params object[] args) {
        Debug.LogError(FormatMessage(message, args));
    }

    public bool editorInitialized = false;

    public void Awake() {
        if (Application.isPlaying) {
            PlayModeAwake();
            return;
        }

        if (!editorInitialized) {
            EditorAwake();
            editorInitialized = true;
        }
    }

    protected virtual void EditorAwake() { }
    protected virtual void PlayModeAwake() { }

    public void Update() {
        if (!Application.isPlaying) {
            EditorUpdate();
        } else {
            PlayModeUpdate();
        }
    }

    protected virtual void EditorUpdate() { }
    protected virtual void PlayModeUpdate() { }

    public virtual void Refresh() { }
    public virtual bool GetAnimationStateEvents() {
        return false;
    }

    /// <summary>
    /// A simple wrapper so that it differentiates code if on editor on not.
    /// This is because we have to delete gameObjects differently depending on 
    /// whether we are on EDITOR mode or on GAME mode.
    /// This method aims to unify the flow.
    /// </summary>
    /// <param name="gameObject"></param>
    protected void DestroyChildGameObject(GameObject gameObject) {
#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
    }

    //public virtual void ReceiveMessage(Message message) {
    //    LogWarning("Received message in base class");
    //}

    public virtual void ReceiveMessage<T>(T msg, object payload = null) where T : IConvertible { 
        LogWarning("Received message in base class");
    }

    public virtual void AnimationStateChange(AnimationStateEvent animationEvent, int stateValue) {
        LogWarning("Received Animation State change in base class");
    }

}