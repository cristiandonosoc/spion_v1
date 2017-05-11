using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Vector3 extensions to make life a little easier
/// </summary>
public static class Vector3Extensions {
    /// <summary>
    /// Only change the X coordinate of the Vector
    /// </summary>
    /// <param name="v">The Vector to change the X coords</param>
    /// <param name="x">The new X coord</param>
    /// <returns>A new vector with the X coord change</returns>
    public static Vector3 WithX(this Vector3 v, float x) {
        return new Vector3(x, v.y, v.z);
    }

    /// <summary>
    /// Only change the Y coordinate of the Vector
    /// </summary>
    /// <param name="v">The Vector to change the Y coords</param>
    /// <param name="y">The new Y coord</param>
    /// <returns>A new vector with the Y coord change</returns>
    public static Vector3 WithY(this Vector3 v, float y) {
        return new Vector3(v.x, y, v.z);
    }

    /// <summary>
    /// Only change the Y coordinate of the Vector
    /// </summary>
    /// <param name="v">The Vector to change the Y coords</param>
    /// <param name="z">The new Y coord</param>
    /// <returns>A new vector with the Y coord change</returns>
    public static Vector3 WithZ(this Vector3 v, float z) {
        return new Vector3(v.x, v.y, z);
    }
}


public static class VectorHelpers {
    public enum CoordChange {
        NO_CHANGE,
        X,
        Y,
        Z
    }

    private static float SnapByFactor(float original, float snappingFactor) {
        int factor = (int)(original / snappingFactor);
        return snappingFactor * factor;
    }

    public static void SnapVector(ref Vector2 v, bool snapX = true, bool snapY = true, float snappingFactor = 1) {
        if (snapX) { v.x = SnapByFactor(v.x, snappingFactor); }
        if (snapY) { v.y = SnapByFactor(v.y, snappingFactor); }
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

    public static void SnapVector(ref Vector3 v, bool snapX = true, bool snapY = true, bool snapZ = true, float snappingFactor = 1) {
        if (snapX) { v.x = SnapByFactor(v.x, snappingFactor); }
        if (snapY) { v.y = SnapByFactor(v.y, snappingFactor); }
        if (snapZ) { v.z = SnapByFactor(v.z, snappingFactor); }
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
