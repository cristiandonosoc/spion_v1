using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceManagerBehaviour : SingletonBehaviour<InstanceManagerBehaviour> {

    #region UI

    public ModelBehaviour defaultFloorPanel;
    public Vector3 defaultFloorPanelCount;
    public bool createFloorWithRoom = true;

    public ModelBehaviour defaultWall;
    public Vector3 defaultWallCount;
    public bool createWallsWithRoom = true;

    #endregion UI

    // Singleton guarantee
    protected InstanceManagerBehaviour() { }

    #region ACTIONS

    public void AddRoom() {
        // We count how many rooms we have
        int roomCount = GetComponentsInChildren<RoomBehaviour>().Length;
        var room = new GameObject().AddComponent<RoomBehaviour>();
        room.name = "Room_" + roomCount;
        room.transform.parent = transform;

        //// 
        //room.defaultFloorPanel = defaultFloorPanel;
        //room.defaultFloorPanelCount = defaultFloorPanelCount;
        //if (createFloorWithRoom) {
        //    //room.AddFloor();
        //}

        //room.defaultWall = defaultWall;
        //room.defaultWallCount = defaultWallCount;
        //if (createWallsWithRoom) {
        //    for (int i = 0; i < 4; i++) {
        //        WallBehaviour wall = room.AddWall(i);
        //        wall.CalculatePrefabs(recreate: true);
        //    }
        //}
    }

    #endregion ACTIONS
}
