using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIDManagerBehaviour : SingletonBehaviour<GUIDManagerBehaviour> {

    private object _lock = new object();

    public uint currentGUID = 1;

    public uint GetGUID() {
        uint GUID = 0;
        lock (_lock) {
            GUID = currentGUID;
            currentGUID++;
        }
        return GUID;
    }
}
