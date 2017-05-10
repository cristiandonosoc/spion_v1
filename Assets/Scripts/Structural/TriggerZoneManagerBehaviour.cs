using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StringStringPair {
    public string key;
    public string value;
}

[Serializable]
public class TriggerMapping {
    [Serializable]
    public class Data {
        public string key;
        public TriggerZoneBehaviour triggerZone;
        public string enterTrigger;
        public string leaveTrigger;
    }
    [SerializeField]
    private Data _data;

    public TriggerMapping(TriggerZoneBehaviour triggerZone) {
        _data = new Data();
        TriggerZone = triggerZone;
    }

    public string Key {
        get { return _data.key; }
    }

    public TriggerZoneBehaviour TriggerZone {
        get { return _data.triggerZone; }
        set {
            _data.triggerZone = value;
            _data.key = _data.triggerZone.name;
        }
    }

}

public delegate void TriggerZoneDelegate(Collider collider);


public class TriggerZoneManagerBehaviour : MonoBehaviour {

    [SerializeField]
    private List<TriggerMapping> _triggerMappings;
    public List<TriggerMapping> TriggerMappings {
        get {
            if (_triggerMappings == null) {
                _triggerMappings = new List<TriggerMapping>();
            }
            return _triggerMappings;
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

        TriggerMappings.Add(new TriggerMapping(zone));

        return zone;
    }

    #endregion ACTIONS

    public void RefreshTriggerZones() {

    }


}
