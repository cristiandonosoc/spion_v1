using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBehaviour<T> : CustomMonoBehaviour where T : CustomMonoBehaviour {
    private static T _instance;
    private static object _lock = new object();

    private static bool applicationIsQuitting = false;

    private void OnEnable() {
        if (Application.isPlaying) {
            applicationIsQuitting = false;
        }
    }
    
    private void OnDestroy() {
        if (Application.isPlaying) {
            applicationIsQuitting = true;
        }
    }

    public static T Instance {
        get {
            if (Application.isPlaying && applicationIsQuitting) {
                LogError("Already destroyed on Application Quit. Returning null.");
                return null;
            }
            lock (_lock) {
                if (_instance == null) {
                    var instances = FindObjectsOfType<T>();
                    if (instances.Length > 1) {
                        LogError("There should only be 1 singleton!");
                        return null;
                    } else if (instances.Length == 1) {
                        Log("Returned already created");
                        _instance = instances[0];
                    } else {
                        _instance = new GameObject("Singleton<" + typeof(T) + ">").AddComponent<T>();
                        //DontDestroyOnLoad(_instance);
                        Log("Created with DontDestroyOnLoad");
                    }
                }
                return _instance;
            }
        }
    }

    private static void Log(string message) {
        Debug.Log("Singleton<" + typeof(T) + ">: " + message);
    }

    private static void LogError(string message) {
        Debug.LogError("Singleton<" + typeof(T) + ">: " + message);
    }

    private static void LogWarning(string message) {
        Debug.LogWarning("Singleton<" + typeof(T) + ">: " + message);
    }

    protected override void EditorAwake() {
        // Do nothing
    }

    protected override void PlayModeAwake() {
        // Do nothing
    }
}