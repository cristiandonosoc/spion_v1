using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomDebug {

    public static class LineHelpers {

        private static object _lock = new object();

        private static Texture2D _texture;
        private static Texture2D Texture {
            get {
                lock (_lock) {
                    if (_texture == null) {
                        _texture = new Texture2D(1, 1);
                        _texture.SetPixel(0, 0, Color.white);
                        _texture.Apply();
                    }
                    return _texture;
                }
            }
        }

        // TODO(Cristian): Investigate why it screws up with not integer points...
        public static void DrawLine(Vector2 v1, Vector2 v2, Color color, float width = 1f) {
            VectorHelpers.SnapVector(ref v1);
            VectorHelpers.SnapVector(ref v2);

            var matrix = GUI.matrix;

            // Store current GUI color, so we can switch it back later,
            // and set the GUI color to the color parameter
            var savedColor = GUI.color;
            GUI.color = color;

            // Determine the angle of the line.
            var angle = Vector3.Angle(v2-v1, Vector2.right);

            // Vector3.Angle always returns a positive number.
            // If v2 is above v1, then angle needs to be negative.
            if (v1.y > v2.y) { angle = -angle; }

            // Use ScaleAroundPivot to adjust the size of the line.
            // We could do this when we draw the texture, but by scaling it here we can use
            //  non-integer values for the width and length (such as sub 1 pixel widths).
            // Note that the pivot point is at +.5 from v1.y, this is so that the width of the line
            //  is centered on the origin at v1.
            GUIUtility.ScaleAroundPivot(new Vector2((v2 - v1).magnitude, width), new Vector2(v1.x, v1.y + 0.5f));

            // Set the rotation for the line.
            //  The angle was calculated with v1 as the origin.
            GUIUtility.RotateAroundPivot(angle, v1);

            // Finally, draw the actual line.
            // We're really only drawing a 1x1 texture from v1.
            // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
            //  render with the proper width, length, and angle.
            GUI.DrawTexture(new Rect(v1.x, v1.y, 1, 1), Texture);

            GUI.matrix = matrix;
        }

        public static void DrawCross(Vector2 center, int length, Color color, float width = 4f) {
            DrawLine(new Vector2(center.x - length, center.y), new Vector2(center.x + length, center.y),
                     color, width);
            DrawLine(new Vector2(center.x, center.y - length), new Vector2(center.x, center.y + length),
                     color, width);
        }
    }
}
