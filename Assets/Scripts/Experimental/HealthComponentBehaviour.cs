using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponentBehaviour : CustomMonoBehaviour {

    [MessageKindMarker]
    public enum Messages {
        NO_HEALTH
    }

    [Serializable]
    public class Data {
        public int maxHP;
        public int currentHP;
    }
    [SerializeField]
    private Data _data;

    public Data Dataz {
        get {
            if (_data == null) {
                _data = new Data();
            }
            return _data;
        }
    }

    public int MaxHP {
        get { return Dataz.maxHP; }
        set { Dataz.maxHP = value; }
    }

    public int CurrentHP {
        get { return Dataz.currentHP; }
        set {
            Dataz.currentHP = value;
            if (Dataz.currentHP <= 0) {
                if (_owner != null) {
                    _owner.ReceiveMessage(Messages.NO_HEALTH);
                }
            }
        }
    }

    private EntityMonoBehaviour _owner;

    protected override void PlayModeAwake() {
        _owner = GetComponent<EntityMonoBehaviour>();
    }


}
