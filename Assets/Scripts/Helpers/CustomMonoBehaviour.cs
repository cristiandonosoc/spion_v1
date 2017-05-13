using UnityEngine;

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

    public virtual void ReceiveMessage(string MessageKind) { }

    public virtual void AnimationStateChange(AnimationStateEvent animationEvent, int stateValue) { }

}