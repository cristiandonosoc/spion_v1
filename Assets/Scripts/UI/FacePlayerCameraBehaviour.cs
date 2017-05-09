using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayerCameraBehaviour : MonoBehaviour {

    public PlayerCameraBehaviour _playerCamera;

    public void Awake() {
        _playerCamera = FindObjectOfType<PlayerCameraBehaviour>();
    }

    public void Update() {
        // We rotate so it always looks to the camera
        Vector3 v = _playerCamera.transform.position - transform.position;
        v.x = 0;
        v.z = 0;

        transform.LookAt(_playerCamera.transform.position - v);
        transform.Rotate(0, 180, 0);
    }

}
