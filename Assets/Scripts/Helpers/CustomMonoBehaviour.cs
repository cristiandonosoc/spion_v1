using UnityEngine;

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
}

//public abstract class InstanceMonoBehaviour : CustomMonoBehaviour {
//    public abstract Instance Model { get; }
//    public abstract void SetModel(Transform parent, Instance model);
//    public abstract Type InstanceType { get; }
//    public abstract int UID { get; }

//    /// <summary>
//    /// IMPORTANT(Cristian): Major hack that enables to detect whether the onDestroy 
//    /// is called by an explicit action, or when Unity destroys everything to 
//    /// running/stopping the app.
//    /// </summary>
//    protected bool IsAppTransitioning {
//        get { return (Time.frameCount == 0) && (Time.renderedFrameCount == 0); }
//    }
//}
