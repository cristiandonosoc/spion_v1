using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a Type that will survive Unity's serialization.
/// Basically it uses a string as the element that gets stored and
/// reconstruct the Type from it upon deserialization.
/// 
/// It supports several operator overloads so that it can be used 
/// interchangebly with types.
/// 
/// This is heavily used on the Messaging API, as it uses relies a lot
/// on RTTI (Run-time Type Information) for correctly knowing what kind
/// of message is received.
/// </summary>
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

    // This operator enables us to do SerializeType t = typof(<class>);
    public static implicit operator SerializableType(Type t) {
        return new SerializableType(t);
    }

    public override bool Equals(object obj) {
        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override string ToString() {
        return type.ToString();
    }

    #region SERIALIZATION

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

    #endregion SERIALIZATION

    #region OPERATORS

    public static bool operator ==(SerializableType t1, SerializableType t2) {
        return t1.type == t2.type;
    }

    public static bool operator !=(SerializableType t1, SerializableType t2) {
        return t1.type != t2.type;
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

    #endregion OPERATORS
}

