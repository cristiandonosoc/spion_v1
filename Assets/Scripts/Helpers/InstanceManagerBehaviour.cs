using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceManagerBehaviour : SingletonBehaviour<InstanceManagerBehaviour> {

    #region UI

    public ModelBehaviour defaultBlockModel;
    public AnimatedModelBehaviour defaultDoorModel;

    #endregion UI

    // Singleton guarantee
    protected InstanceManagerBehaviour() { }

    #region ACTIONS

    public RoomBehaviour AddRoom() {
        // We count how many rooms we have
        int roomCount = GetComponentsInChildren<RoomBehaviour>().Length;
        var room = new GameObject().AddComponent<RoomBehaviour>();
        room.name = "Room_" + roomCount;
        room.transform.parent = transform;
        return room;
    }

    #endregion ACTIONS
}
