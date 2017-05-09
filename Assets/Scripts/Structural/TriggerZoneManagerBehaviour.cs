using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StringStringPair {
    public string key;
    public string value;
}

public delegate void TriggerZoneDelegate(Collider collider);


public class TriggerZoneManagerBehaviour : MonoBehaviour {

    public List<StringStringPair> strings;


    #region ACTIONS

    public TriggerZoneBehaviour AddTriggerZone() {
        // We count hoy many trigger zones we have
        int zoneCount = GetComponentsInChildren<TriggerZoneBehaviour>().Length;
        TriggerZoneBehaviour zone = new GameObject().AddComponent<TriggerZoneBehaviour>();
        zone.name = "TriggerZone_" + zoneCount;
        zone.transform.parent = transform;
        zone.transform.localPosition = Vector3.zero;

        return zone;
    }

    #endregion ACTIONS


}
