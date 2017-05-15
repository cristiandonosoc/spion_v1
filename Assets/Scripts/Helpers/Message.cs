using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// This is the main Message mechanism to be used within the game.
/// The idea is to send messages with this format to CustomMonoBehaviours.
/// The idea behind this is the following:
///     This enables decoupling between elements, as before a calling object
///     had to call a method from the receiver. This binds the receiver to never
///     change that method.
///     With this mechanism, the receiver never has to know who sends a message.
///     He just defines a Message interface and everyone can send messages.
///     If a message should require identification, all the necessary information
///     must be included within the message payload.
///
///     The initial pattern is as follows: 
///         1.  An object defines his MessageKind enum and marks it with [MessageKindMarker]
///             attribute. This attribute is not needed, but some functionality depends on it
///             in order to be discoverable in the editor. See TriggerZoneManagerBehaviour for more information.
///             This basically defines what kind of messages "API" this object defines.
///         2.  An object can define a MessagePayload struct, which describes what extra
///             information this object expects to see with the message.
///     With both those elements, the message can describe his messaging API.
///     
/// The Message class work as follows:
/// A message consists of three parts:
/// 1. A Type: 
///     Messages are basically enums with more information. The extra information is
///     stored in the payload (See below). As an object can receive many different kinds
///     of messages, we need a field to tells use to what enum this message belongs to.
///     With this information we can correctly interpret (cast) the messageKind field and
///     know what the message is trying to tell.
///     It is implemented as a SerializedType. The idea is that the type information 
///     survives serializing. See SerializedType for more information.
/// 2. MessageKind: 
///     This is simply an int that will be casted to the type kind. 
///     This identifies what the message is.
/// 3. Payload (optional):
///     The payload is a object defined object that goes with the message.
///     As Message is a system that will be used for all the messages, an object is
///     used to store the payload and it will be casted to correct format upon arrival
///     and correct decoding of the Type/MessageKind information.
///     As Unity won't serialize object types, upon serialization, this class stores
///     the information in a byte[] that will be serialized by unity. The it will
///     again interpret those bytes as the correct payload.
///     This makes the Message class completely serializable.
/// </summary>
[Serializable]
public class Message : ISerializationCallbackReceiver {
    public SerializableType type;
    public int messageKind;
    public object payload;

    // We require this for serialization
    [SerializeField]
    private byte[] _serializedPayload;

    /// <summary>
    /// Generic call to create a Message.
    /// In general, Prefer the Factory Create method, as it is much less verbose.
    /// </summary>
    /// <param name="type">The type of the message. Should be a MessageKindMark attributed enum.</param>
    /// <param name="messageKind">Enum value associated with the message type. Will be casted to int internally.</param>
    /// <param name="messageObject">
    /// A generic payload associated with the message. Can be null.
    /// The receiver has to be able to correclty cast it back from object.
    /// </param>
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
    /// <returns>A valid Message Object</returns>
    public static Message Create<T>(T messageKind, object messageObject = null) {
        return new Message(typeof(T), (int)(object)messageKind, messageObject);
    }

    #region SERIALIZATION

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

    #endregion SERIALIZATION
}

