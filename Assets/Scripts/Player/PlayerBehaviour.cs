using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    [StateMachineEnum]
    public enum States {
        NORMAL = 0,
        DASH = 1
    }

    [StateMachineEnum]
    public enum TestEnum {
        TEST

    }

    public float speed = 0.1f;
    public Vector3 move = new Vector3(0, 0, -1);
    public Vector3 target;
    public float targetDistance = 10;
    public float gravitySpeed = 0.1f;


    public int maxHP = 10;
    public int currentHP = 10;


    public Vector3 TargetPosition {
        get { return transform.position + target; }
    }

    private CharacterController _characterController;
    Camera _playerCamera;

    public void Awake() {
        _characterController = GetComponent<CharacterController>();
        _playerCamera = FindObjectOfType<PlayerCameraBehaviour>().GetComponent<Camera>();
    }

    public void Start() {
        LookAtMove(doMove: false);
    }

    void Update() {
        Debug.DrawLine(transform.position, transform.position + 10 * move.normalized);

        UpdateGravity();
        if (UpdateMove()) {
            LookAtMove(doMove: true);
        }
    }

    private void UpdateGravity() {
        Vector3 gravity = new Vector3(0, -gravitySpeed, 0);
        _characterController.Move(gravity);
    }

    private bool UpdateMove() {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        var rx = Input.GetAxisRaw("Horizontal");
        var rz = Input.GetAxisRaw("Vertical");
        if (rx == 0) { x /= 3; }
        if (rz == 0) { z /= 3; }

        bool change = false;
        if ((x != 0) || (z != 0)) {
            move = new Vector3(x, 0, z) * speed;
            if (move.sqrMagnitude > 1) {
                move.Normalize();
            }
            Vector3 cameraEuler = _playerCamera.transform.rotation.eulerAngles;
            Quaternion cameraRotation = Quaternion.Euler(0, cameraEuler.y, 0);
            move = cameraRotation * move;

            change = true;
        }
        target = move.normalized * targetDistance;
        return change;
    }

    private void LookAtMove(bool doMove = true) {
        transform.rotation = Quaternion.LookRotation(move);
        if (doMove) {
            //transform.Translate(move);
            _characterController.Move(move);
        }
    }
}