using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


[Serializable]
public struct SerializableType : ISerializationCallbackReceiver {
    public Type type;

    [SerializeField]
    private string _typeString;

    public SerializableType(Type type) {
        this.type = type;
        if (type != null) {
            _typeString = type.FullName;
        } else {
            _typeString = null;
        }
    }

    public static implicit operator SerializableType(Type t) {
        return new SerializableType(t);
    }

    public static bool operator ==(SerializableType t1, SerializableType t2) {
        return t1.type == t2.type;
    }

    public static bool operator !=(SerializableType t1, SerializableType t2) {
        return t1.type != t2.type;
    }

    public override bool Equals(object obj) {
        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public void OnBeforeSerialize() {
        if (type != null) {
            _typeString = type.FullName;
        } else {
            _typeString = null;
        }
    }

    public void OnAfterDeserialize() {
        if (_typeString != null) {
            type = Type.GetType(_typeString);
        } else {
            type = null;
        }
    }

    public static bool operator ==(SerializableType st, Type t) {
        bool result = st.type == t;
        return result;
    }

    public static bool operator !=(SerializableType st, Type t) {
        return st.type != t;
    }

    public static bool operator ==(Type t, SerializableType st) {
        bool result = st.type == t;
        return result;
    }

    public static bool operator !=(Type t, SerializableType st) {
        return st.type != t;
    }

    public override string ToString() {
        return type.ToString();
    }
}

[Serializable]
public struct Message : ISerializationCallbackReceiver {
    public SerializableType type;
    public int messageKind;
    public object payload;

    // We require this for serialization
    [SerializeField]
    private byte[] _serializedPayload;

    public Message(Type type, int messageKind, object payload) {
        this.type = type;
        this.messageKind = messageKind;
        this.payload = payload;
        _serializedPayload = null;
    }

    /// <summary>
    /// Creates a Message Object of a certain Type. The type is the class supposed
    /// to receive the message. All the kinds are payload are generically serialized
    /// so that anyone can use this system.
    /// This means that both the sender and the receiver have to be able to correctly
    /// cast the message.
    /// </summary>
    /// <typeparam name="MessageKindMarker">The Type of the enum associated with this message.</typeparam>
    /// <param name="messageKind">Enum value associated with the message type. Will be casted to int internally.</param>
    /// <param name="messageObject">
    /// A generic payload associated with the message. Can be null.
    /// The receiver has to be able to correclty cast it back from object.
    /// </param>
    /// <returns></returns>
    public static Message Create<T>(object messageKind, object messageObject = null) {
        return new Message(typeof(T), (int)messageKind, messageObject);
    }

    public void OnBeforeSerialize() {
        if (payload == null) {
            _serializedPayload = null;
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream()) {
            bf.Serialize(ms, payload);
            _serializedPayload = ms.ToArray();
        }
    }

    public void OnAfterDeserialize() {
        if ((_serializedPayload == null) || 
            (_serializedPayload.Length == 0)) {
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream()) {
            ms.Write(_serializedPayload, 0, _serializedPayload.Length);
            payload = bf.Deserialize(ms);
        }
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