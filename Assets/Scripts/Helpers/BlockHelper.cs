using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockHelper {

    public static void SnapToFloor(BlockBehaviour block) {
        var room = block.transform.parent.GetComponent<RoomBehaviour>();

        float floorHeight = 1;
        block.transform.localPosition = new Vector3(0, (-room.Extents.y + floorHeight / 2), 0);
        block.Size = new Vector3(room.Size.x, floorHeight, room.Size.z);
        block.name = "Floor";
        block.Refresh();
    }

    public static void SnapToRoom(BlockBehaviour block, int wallIndex) {
        // TODO(Cristian): Should snapping consider floor (for initial height)?
        // TODO(Cristian): Should walls be outside the room (minor 0.1f)?
        var room = block.transform.parent.GetComponent<RoomBehaviour>();

        float wallWidth = 1;

        if (wallIndex == 0) {   // NORTH
            block.transform.localPosition = new Vector3(0, 0, room.Extents.z - wallWidth / 2);
            //transform.rotation = Quaternion.Euler(0, 180, 0);
            block.Size = new Vector3(room.Size.x, room.Size.y, wallWidth);
            block.name = "North Wall";
        } else if (wallIndex == 1) {    // EAST
            //block.transform.localPosition = new Vector3(room.Size.x / 2, 0, 0);
            block.transform.localPosition = new Vector3(room.Extents.x  - wallWidth / 2, 0, 0);
            //transform.rotation = Quaternion.Euler(0, 270, 0);
            block.Size = new Vector3(wallWidth, room.Size.y, room.Size.z);
            block.name = "East Wall";
        } else if (wallIndex == 2) {    // SOUTH
            block.transform.localPosition = new Vector3(0, 0, -room.Extents.z + wallWidth / 2);
            //transform.rotation = Quaternion.identity;
            block.Size = new Vector3(room.Size.x, room.Size.y, wallWidth);
            block.name = "South Wall";
        } else if (wallIndex == 3) {    // WEST
            block.transform.localPosition = new Vector3(-room.Extents.x + wallWidth / 2, 0, 0);
            //transform.rotation = Quaternion.Euler(0, 90, 0);
            block.Size = new Vector3(wallWidth, room.Size.y, room.Size.z);
            block.name = "West Wall";
        } else {
            return;
        }

        block.Refresh();
    }
}
