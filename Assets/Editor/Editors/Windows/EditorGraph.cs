using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class EditorGraph : EditorWindow {

    [MenuItem("Window/Graph")]
    public static void ShowGraph() {
        EditorWindow.GetWindow<EditorGraph>();
    }

    Material _lineMaterial;

    void OnEnable() {
        EditorApplication.update += PrivateDelegate;
    }

    void OnDisable() {
        EditorApplication.update -= PrivateDelegate;

    }

    void PrivateDelegate() {
        Repaint();
    }


    void CreateMaterial() {
        _lineMaterial = new Material(Shader.Find("Custom/GraphShader"));
        _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        _lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
    }

    private const int controlsWidth = 300;
    private const int margin = 10;
    private const int graphBegin = 350;

    void OnGUI() {
        int width = (int)position.width;
        int height = (int)position.height;

        Rect controlsArea = new Rect(margin, margin,
                                     controlsWidth - margin, height - margin);

        CustomDebug.Graph debugGraph = CustomDebug.Graph.Instance;
        CustomDebug.GraphChannelSet channelSet = debugGraph.GetChannelSet();

        GUILayout.BeginArea(controlsArea);

        foreach (CustomDebug.GraphChannel channel in channelSet.Channels) {
            EditorGUILayout.BeginHorizontal();
            channel.Active = EditorGUILayout.Toggle(channel.Name, channel.Active);
            channel.ChannelColor = EditorGUILayout.ColorField(channel.ChannelColor);
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndArea();

        float yMin = channelSet.ActiveMin;
        float yMax = channelSet.ActiveMax;

    

        EditorGUI.LabelField(new Rect(controlsWidth + margin, 8, 40, 20), yMax.ToString());
        EditorGUI.LabelField(new Rect(controlsWidth + margin, height - margin - 12, 40, 20), yMin.ToString());

        if (Event.current.type != EventType.Repaint) {
            return;
        }

        if (debugGraph.Size == 0) {
            return;
        }

        if (_lineMaterial == null) {
            CreateMaterial();
        }
        _lineMaterial.SetPass(0);


        GL.PushMatrix();
        GL.LoadPixelMatrix();

        GL.Begin(GL.LINES);

        GL.Color(Color.black);
        DrawSquare(new Vector3(graphBegin, margin),
                   new Vector3(width - margin, height - margin));


        // No sense in graphing empty
        if (yMin != yMax) {
            foreach (CustomDebug.GraphChannel channel in channelSet.Channels) {
                if ((channel == null) || (!channel.Active)) {
                    continue;
                }

                GL.Color(channel.ChannelColor);

                int i = 0;
                int xPix = int.MaxValue;
                bool firstPass = true;
                float pastX = 0;
                float pastY = 0;
                IEnumerator reverseEnumerator = channel.GetReverseEnumerator();
                while (reverseEnumerator.MoveNext() && (xPix > graphBegin)) {
                    xPix = (width - margin) - i;
                    float y = (float)reverseEnumerator.Current;
                    float y_01 = Mathf.InverseLerp(yMin, yMax, y);
                    int yPix = (margin + 1) + (int)(y_01 * (height - 2 * margin - 1));
                    if (firstPass) {
                        firstPass = false;
                    } else {
                        Plot(pastX, pastY, xPix, yPix);
                    }
                    pastX = xPix;
                    pastY = yPix;
                    i++;
                }
            }
        }

        GL.End();
        GL.PopMatrix();
    }

    void Plot(float x, float y) {
        GL.Vertex3(x - 1, y - 1, 0);
        GL.Vertex3(x - 1, y + 1, 0);
        GL.Vertex3(x + 1, y - 1, 0);
        GL.Vertex3(x + 1, y + 1, 0);
    }

    void Plot(float x1, float y1, float x2, float y2) {
        GL.Vertex3(x1, y1, 0);
        GL.Vertex3(x2, y2, 0);
    }

    void DrawSquare(Vector3 v1, Vector3 v2) {
        GL.Vertex3(v1.x, v1.y, v1.z);
        GL.Vertex3(v2.x, v1.y, v1.z);

        GL.Vertex3(v2.x, v1.y, v2.z);
        GL.Vertex3(v2.x, v2.y, v2.z);

        GL.Vertex3(v2.x, v2.y, v2.z);
        GL.Vertex3(v1.x, v2.y, v2.z);

        GL.Vertex3(v1.x, v2.y, v2.z);
        GL.Vertex3(v1.x, v1.y, v1.z);
    }






}
