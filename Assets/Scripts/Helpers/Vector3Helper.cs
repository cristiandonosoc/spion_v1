using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorHelpers {

    public enum CoordChange {
        NO_CHANGE,
        X,
        Y,
        Z
    }

    public static void SnapVector(ref Vector2 v, bool snapX = true, bool snapY = true) {
        if (snapX) { v.x = (int)v.x; }
        if (snapY) { v.y = (int)v.y; }
    }

    public static Vector2 SnapToAngle(Vector2 v, float snap) {
        float angle = Vector2.Angle(v, new Vector2(1, 0));
        if (v.y < 0) { angle = -angle; }
        float snappedAngle = Mathf.RoundToInt(angle / snap) * snap;
        var q = Quaternion.AngleAxis(angle - snappedAngle, Vector3.up);
        Vector3 t = new Vector3(v.x, 0, v.y);
        t = q * t;
        return new Vector2(t.x, t.z);
    }

    public static void SnapVector(ref Vector3 v, bool snapX = true, bool snapY = true, bool snapZ = true) {
        if (snapX) { v.x = (int)v.x; }
        if (snapY) { v.y = (int)v.y; }
        if (snapZ) { v.z = (int)v.z; }
    }


    public static Vector3 Multiply(Vector3 a, Vector3 b) {
        return new Vector3(a.x * b.x,
                           a.y * b.y,
                           a.z * b.z);
    }

    public static Vector3 Divide(Vector3 a, Vector3 b) {
        return new Vector3(a.x / b.x,
                           a.y / b.y,
                           a.z / b.z);
    }

    public static Vector3 RotateAroundPivot(Vector3 target, Vector3 pivot, Quaternion rotation) {
        Vector3 dir = target - pivot;   // We get the direction relative to pivot
        dir = rotation * dir;           // Rotate it
        return dir + pivot;
    }

    public static Vector3 RotateAroundOrigin(Vector3 target, Quaternion rotation) {
        return rotation * target;
    }

    public static Vector3 ChangeCoords(Vector3 target, CoordChange XChange, CoordChange YChange, CoordChange ZChange) {
        Vector3 result = target;
        if (XChange != CoordChange.NO_CHANGE) {
            result.x = GetCoordFromCoordChange(target, XChange);
        }
        if (YChange != CoordChange.NO_CHANGE) {
            result.y = GetCoordFromCoordChange(target, YChange);
        }
        if (ZChange != CoordChange.NO_CHANGE) {
            result.z = GetCoordFromCoordChange(target, ZChange);
        }

        return result;
    }

    public static float GetCoordFromCoordChange(Vector3 target, CoordChange coordChange) {
        switch (coordChange) {
            case CoordChange.X:
                return target.x;
            case CoordChange.Y:
                return target.y;
            case CoordChange.Z:
                return target.z;
            default:
                throw new System.InvalidProgramException("Invalid CoordChange");
        }
    }


}
