using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


[Serializable]
public class TriggerMapping {
    [Serializable]
    public class Data {
        public string key;
        public TriggerZoneBehaviour triggerZone;

        public string enterTrigger;
        public Type enterType;
        public MethodInfo enterMethodInfo;

        public string exitTrigger;
        public Type exitType;
        public MethodInfo exitMethodInfo;
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

    public string EnterTrigger {
        get { return _data.enterTrigger; }
        set {
            _data.enterTrigger = value;
        }
    }

    public Type EnterType {
        get { return _data.enterType; }
    }

    public MethodInfo EnterMethodInfo {
        get { return _data.enterMethodInfo; }
    }

    public string ExitTrigger {
        get { return _data.exitTrigger; }
        set { _data.exitTrigger = value; }
    }

    public Type ExitType {
        get { return _data.exitType; }
    }

    public MethodInfo ExitMethodInfo {
        get { return _data.exitMethodInfo; }
    }

    /// <summary>
    /// Has to be done on runtime because (apparently),
    /// Unity won't serialize Type and MethodInfo
    /// </summary>
    public void CompileTypeAndMethodInfos() {
        string[] split = _data.enterTrigger.Split('|');
        _data.enterType = Type.GetType(split[0]);
        _data.enterMethodInfo = _data.enterType.GetMethod(split[1]);

        split = _data.exitTrigger.Split('|');
        _data.exitType = Type.GetType(split[0]);
        _data.exitMethodInfo = _data.exitType.GetMethod(split[1]);
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
        TriggerZoneBehaviour[] zones = GetComponentsInChildren<TriggerZoneBehaviour>();
        List<TriggerMapping> validMappings = new List<TriggerMapping>();
        foreach(TriggerMapping mapping in TriggerMappings) {
            bool found = false;
            foreach (TriggerZoneBehaviour zone in zones) {
                if (mapping.TriggerZone == zone) {
                    // NOTE(Cristian): Will rewrite name (redundant... I know)
                    mapping.TriggerZone = zone; 
                    found = true;
                    break;
                }

                if (mapping.Key == zone.name) {
                    mapping.TriggerZone = zone;
                    found = true;
                    break;
                }
            }

            if (found) {
                validMappings.Add(mapping);
            }
        }

        _triggerMappings = validMappings;
    }

    public void Awake() {
        Debug.Log("Trigger Behaviour awake");
        CompileTriggers();
    }

    private void CompileTriggers() {
        Component[] components = GetComponents<CustomMonoBehaviour>();
        TriggerZoneBehaviour[] triggerZones = GetComponentsInChildren<TriggerZoneBehaviour>();
        foreach (TriggerMapping mapping in TriggerMappings) {
            mapping.CompileTypeAndMethodInfos();
            foreach (Component component in components) {
                Type componentType = component.GetType();
                if (componentType == mapping.EnterType) {
                    var enterDelegate = (TriggerZoneDelegate)Delegate.CreateDelegate(typeof(TriggerZoneDelegate), 
                                                                                     component, 
                                                                                     mapping.EnterMethodInfo);
                    foreach (TriggerZoneBehaviour triggerZone in triggerZones) {
                        if (triggerZone.name == mapping.Key) {
                            triggerZone.EnterDelegate = enterDelegate;
                            break;
                        }
                    }
                }
                if (componentType == mapping.ExitType) {
                    var exitDelegate = (TriggerZoneDelegate)Delegate.CreateDelegate(typeof(TriggerZoneDelegate),
                                                                                    component,
                                                                                    mapping.ExitMethodInfo);
                    foreach (TriggerZoneBehaviour triggerZone in triggerZones) {
                        if (triggerZone.name == mapping.Key) {
                            triggerZone.ExitDelegate = exitDelegate;
                            break;
                        }
                    }
                }
            }
        }
    }


}
