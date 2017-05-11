using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayerCameraBehaviour : MonoBehaviour {

    public PlayerCameraBehaviour playerCamera;
    public bool rotateOnlyY = true;

    public void Awake() {
        if (playerCamera == null) {
            playerCamera = FindObjectOfType<PlayerCameraBehaviour>();
        }
    }

    public void Update() {
        if (rotateOnlyY) {
            // TODO(Cristian): Do lerp here, instead of piggybacking on the camera's lerp
            Vector3 v = playerCamera.transform.position - transform.position;
            v.x = 0;
            v.z = 0;
            transform.LookAt(playerCamera.transform.position - v);
            transform.Rotate(0, 180, 0);
        } else {
            var fwd = Camera.main.transform.forward;
            transform.rotation = Quaternion.LookRotation(fwd);
        }

    }

}
