using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponentBehaviour : CustomMonoBehaviour {

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
        set { Dataz.currentHP = value; }
    }


}
