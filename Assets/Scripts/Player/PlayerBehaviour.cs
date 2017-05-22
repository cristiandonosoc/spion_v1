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
        TEST = 0,
        TEST1 = 1
    }

    public float speed = 0.1f;
    public Vector3 moveDirection = new Vector3(0, 0, -1);
    public Vector3 lookDirection;
    public Vector3 target;
    public float targetDistance = 10;
    public float gravitySpeed = 0.1f;


    public int maxHP = 10;
    public int currentHP = 10;


    public Vector3 TargetPosition {
        get { return transform.position + target; }
    }

    private CharacterController _characterController;
    private Camera _playerCamera;
    private StateMachineBehaviour _stateMachine;

    public void Awake() {
        _characterController = GetComponent<CharacterController>();
        _playerCamera = FindObjectOfType<PlayerCameraBehaviour>().GetComponent<Camera>();
        _stateMachine = GetComponent<StateMachineBehaviour>();

        lookDirection = moveDirection;
    }

    public void Start() {
        LookAtMove();
    }

    void Update() {
        States currentState = _stateMachine.GetCurrentState<States>();

        UpdateControls(currentState);
        UpdateMove(currentState);

        Debug.DrawLine(transform.position, transform.position + 10 * lookDirection.normalized);
    }

    private void UpdateControls(States currentState) {
        if (currentState == States.DASH) {

        } else if (currentState == States.NORMAL) {
            if (Input.GetButtonDown("A")) {
                if (_stateMachine.IsCurrentState(States.NORMAL)) {
                    _stateMachine.ChangeState(States.DASH);
                }
            } else {
                moveDirection = Vector3.zero;

                var x = Input.GetAxis("Horizontal");
                var z = Input.GetAxis("Vertical");

                var rx = Input.GetAxisRaw("Horizontal");
                var rz = Input.GetAxisRaw("Vertical");
                if (rx == 0) { x /= 3; }
                if (rz == 0) { z /= 3; }

                if ((x != 0) || (z != 0)) {
                    moveDirection = new Vector3(x, 0, z) * speed;
                    if (moveDirection.sqrMagnitude > 1) {
                        moveDirection.Normalize();
                    }
                    Vector3 cameraEuler = _playerCamera.transform.rotation.eulerAngles;
                    Quaternion cameraRotation = Quaternion.Euler(0, cameraEuler.y, 0);
                    moveDirection = cameraRotation * moveDirection;
                    lookDirection = moveDirection;
                }
                target = lookDirection.normalized * targetDistance;
            }
        }
    }

    private void UpdateGravity(States currentState) {
        Vector3 gravity = new Vector3(0, -gravitySpeed, 0);
        _characterController.Move(gravity);
    }

    private void UpdateMove(States currentState) {
        UpdateGravity(currentState);
        LookAtMove();


    }

    private void LookAtMove() {
        if (moveDirection != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(moveDirection);
            _characterController.Move(moveDirection);
        }
    }
}