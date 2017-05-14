using System;
using UnityEngine;

public struct Message {
    public Type messageType;
    public object messageValue;

    public Message(Type messageType, object messageObject) {
        this.messageType = messageType;
        this.messageValue = messageObject;
    }

    public static Message Create<T>(object messageObject) {
        return new Message(typeof(T), messageObject);
    }
}

[ExecuteInEditMode]
public abstract class CustomMonoBehaviour : MonoBehaviour {

    public void Log(string message, params object[] args) {
        Debug.Log(name + ": " + string.Format(message, args));
    }

    public void LogError(string message, params object[] args) {
        Debug.LogError(name + ": " + string.Format(message, args));
    }

    public void LogWarning(string message, params object[] args) {
        Debug.LogWarning(name + ": " + string.Format(message, args));
    }

    public bool editorInitialized = false;

    public void Awake() {
        if (!editorInitialized) {
            EditorAwake();
            editorInitialized = true;
        } else {
            PlayModeAwake();
        }
    }

    protected abstract void EditorAwake();
    protected abstract void PlayModeAwake();
    public abstract void Refresh();

    protected void DestroyChildGameObject(GameObject gameObject) {
#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
    }

    public virtual void ReceiveMessage(Message message) {
        LogWarning("Received message in base class");
    }


    public virtual void AnimationStateChange(AnimationStateEvent animationEvent, int stateValue) {
        LogWarning("Received Animation State change in base class");
    }

}