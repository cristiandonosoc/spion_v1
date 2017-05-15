using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MessageKindMarker : Attribute { }

public struct TriggerZoneMessage {
    public enum MessageKind {
        ENTER,
        EXIT
    }

    [Serializable]
    public struct MessagePayload {
        public Collider collider;
        public Message internalMessage;
    } 
}

[Serializable]
public class TriggerZoneBinding {
    [Serializable]
    public class Data {
        public TriggerZoneBehaviour triggerZone;

        public SerializableType enterBindedType;
        public string enterKey;
        public Message enterInternalMessage;

        public SerializableType exitBindedType;
        public string exitKey;
        public Message exitInternalMessage;
    }
    [SerializeField]
    private Data _data;

    public TriggerZoneBinding(TriggerZoneBehaviour triggerZone) {
        _data = new Data();
        TriggerZone = triggerZone;
    }

    public TriggerZoneBehaviour TriggerZone {
        get { return _data.triggerZone; }
        set {
            _data.triggerZone = value;
        }
    }

    // ENTER
    public SerializableType EnterBindedType {
        get { return _data.enterBindedType; }
        set { _data.enterBindedType = value; }
    }

    public string EnterKey {
        get { return _data.enterKey; }
        set {
            _data.enterKey = value;
        }
    }

    public Message EnterInternalMessage {
        get { return _data.enterInternalMessage; }
        set { _data.enterInternalMessage = value; }
    }

    // EXIT
    public SerializableType ExitBindedType {
        get { return _data.exitBindedType; }
        set { _data.exitBindedType = value; }
    }

    public Message ExitInternalMessage {
        get { return _data.exitInternalMessage; }
        set { _data.exitInternalMessage = value; }
    }

    public string ExitKey {
        get { return _data.exitKey; }
        set { _data.exitKey = value; }
    }
}


public delegate void TriggerZoneDelegate(Collider collider);


public class TriggerZoneManagerBehaviour : MonoBehaviour {

    [SerializeField]
    private List<TriggerZoneBinding> _triggerZoneMappings;
    public List<TriggerZoneBinding> TriggerZoneMappings {
        get {
            if (_triggerZoneMappings == null) {
                _triggerZoneMappings = new List<TriggerZoneBinding>();
            }
            return _triggerZoneMappings;
        }
        set {
            _triggerZoneMappings = value;
        }
    }

    #region ACTIONS

    public TriggerZoneBehaviour AddTriggerZone() {
        // We count hoy many trigger zones we have
        int zoneCount = GetComponentsInChildren<TriggerZoneBehaviour>().Length;
        TriggerZoneBehaviour zone = new GameObject().AddComponent<TriggerZoneBehaviour>();
        zone.name = "TriggerZone_" + zoneCount;
        zone.transform.parent = transform;
        zone.transform.localPosition = Vector3.zero;

        TriggerZoneMappings.Add(new TriggerZoneBinding(zone));

        return zone;
    }

    #endregion ACTIONS

    public void Start() {
        CompileTriggers();
    }

    private void CompileTriggers() {

        if (TriggerZoneMappings.Count == 0) { return; }

        // We clear the mappings
        // TODO(Cristian): Is there a non-stupid way to do this?
        TriggerZoneBehaviour[] triggerZones = GetComponentsInChildren<TriggerZoneBehaviour>();
        foreach (TriggerZoneBehaviour triggerZone in triggerZones) {
            triggerZone.EnterReceivers.Clear();
            triggerZone.ExitReceivers.Clear();
        }


        CustomMonoBehaviour[] targets = GetComponents<CustomMonoBehaviour>();
        foreach (TriggerZoneBinding mapping in TriggerZoneMappings) {
            foreach (CustomMonoBehaviour target in targets) {
                Type componentType = target.GetType();
                if (componentType == mapping.EnterBindedType) {
                    mapping.TriggerZone.EnterReceivers.Add(new TriggerZoneMapping(target, mapping.EnterInternalMessage));
                }

                if (componentType == mapping.ExitBindedType) {
                    mapping.TriggerZone.ExitReceivers.Add(new TriggerZoneMapping(target, mapping.ExitInternalMessage));
                }
            }
        }
    }
}
