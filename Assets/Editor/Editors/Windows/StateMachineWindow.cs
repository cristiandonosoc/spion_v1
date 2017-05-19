using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class StateMachineWindow : EditorWindow {

    Rect window1;
    Rect window2;

    int selectedIndex = 0;
    Type[] types;

    public const int controlsWidth = 300;
    public const int margin = 10;

    [MenuItem("Window/State Machine")]
    static void ShowEditor() {
        StateMachineWindow window = GetWindow<StateMachineWindow>();
        window.Init();
    }

    public void Init() {
        window1 = new Rect(10, 10, 100, 100);
        window2 = new Rect(210, 210, 100, 100);
    }

    void OnGUI() {
        DrawControlGUI();
        BeginWindows();
        window1 = GUI.Window(1, window1, DrawNodeWindow, "Window 1");
        window2 = GUI.Window(2, window2, DrawNodeWindow, "Window 2");
        EndWindows();

        Event e = Event.current;
        Vector2 mousePos = e.mousePosition;
        if (e.button == 1) {
            if (e.type == EventType.mouseDown) {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Test"), false, Callback, "test");
                menu.ShowAsContext();
                e.Use();

            }

        }


    }

    void Callback(object obj) {
        Debug.Log(obj.ToString());

    }

    void DrawControlGUI() {
        int width = (int)position.width;
        int height = (int)position.height;

        Rect controlsArea = new Rect(margin, margin,
                                     controlsWidth - margin, height - margin);

        GUILayout.BeginArea(controlsArea);
        if (types == null) {
            types = TypeHelpers.GetAttributedTypes(typeof(StateMachineEnum));
        }
        string[] names = TypeHelpers.GetNames(types);


        EditorGUILayout.Popup("State Machine", 0, names);


        GUILayout.EndArea();

    }

    void DrawNodeWindow(int id) {
        GUI.DragWindow();
    }

}