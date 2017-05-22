using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    [StateMachineEnum]
    public enum States {
        NORMAL = 0,
        DASHING = 1
    }

    #region DATA

    [Serializable]
    public class Data {
        public float speed = 0.1f;
        public Vector3 moveDirection = new Vector3(0, 0, -1);
        public Vector3 lookDirection;

        public float targetDistance = 10;
        public Vector3 targetVector;

        // TODO(Cristian): Do a much better gravity feel
        public float gravitySpeed = 0.1f;

        public int maxHP = 10;
        public int currentHP = 10;

        public Data() {
            lookDirection = moveDirection;
        }
    }
    [SerializeField]
    private Data _data;
    public Data InternalData {
        get {
            if (_data == null) {
                _data = new Data();
            }
            return _data;
        }
    }

    public float Speed {
        get { return InternalData.speed; }
        set { InternalData.speed = value; }
    }
    public float TargetDistance {
        get { return InternalData.targetDistance; }
        set { InternalData.targetDistance = value; }
    }
    public float GravitySpeed {
        get { return InternalData.gravitySpeed; }
        set { InternalData.gravitySpeed = value; }
    }
    public int CurrentHP {
        get { return InternalData.currentHP; }
        set { InternalData.currentHP = value; }
    }
    public int MaxHP {
        get { return InternalData.maxHP; }
        set { InternalData.maxHP = value; }
    }

    public Vector3 MoveDirection { get { return InternalData.moveDirection; } }

    public Vector3 TargetPosition {
        get { return transform.position + InternalData.targetVector; }
    }

    #endregion DATA

    private CharacterController _characterController;
    private Camera _playerCamera;
    private StateMachineBehaviour _stateMachine;

    public void Awake() {
        _characterController = GetComponent<CharacterController>();
        _playerCamera = FindObjectOfType<PlayerCameraBehaviour>().GetComponent<Camera>();
        _stateMachine = GetComponent<StateMachineBehaviour>();
    }

    public void Start() {
        LookAtMove();
    }

    void Update() {
        States currentState = _stateMachine.GetCurrentState<States>();

        UpdateControls(currentState);
        UpdateMove(currentState);

        Debug.DrawLine(transform.position, transform.position + 10 * InternalData.lookDirection.normalized);
    }

    private void UpdateControls(States currentState) {
        if (currentState == States.DASHING) {
            return;
        } else if (currentState == States.NORMAL) {
            if (Input.GetButtonDown("A")) {
                if (_stateMachine.IsCurrentState(States.NORMAL)) {
                    _stateMachine.ChangeState(States.DASHING);
                }
            } else {
                UpdateMoveControls();
            }
        }
    }

    private void UpdateMoveControls() {
        InternalData.moveDirection = Vector3.zero;
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        var rx = Input.GetAxisRaw("Horizontal");
        var rz = Input.GetAxisRaw("Vertical");
        if (rx == 0) { x /= 3; }
        if (rz == 0) { z /= 3; }

        if ((x != 0) || (z != 0)) {
            InternalData.moveDirection = new Vector3(x, 0, z) * InternalData.speed;
            if (InternalData.moveDirection.sqrMagnitude > 1) {
                InternalData.moveDirection.Normalize();
            }
            Vector3 cameraEuler = _playerCamera.transform.rotation.eulerAngles;
            Quaternion cameraRotation = Quaternion.Euler(0, cameraEuler.y, 0);
            InternalData.moveDirection = cameraRotation * InternalData.moveDirection;
            InternalData.lookDirection = InternalData.moveDirection;
        }
        // A cache of the target
        InternalData.targetVector = InternalData.lookDirection.normalized * InternalData.targetDistance;
    }

    private void UpdateGravity(States currentState) {
        Vector3 gravity = new Vector3(0, -InternalData.gravitySpeed, 0);
        _characterController.Move(gravity);
    }

    private void UpdateMove(States currentState) {
        if (currentState == States.NORMAL) {
            UpdateGravity(currentState);
            LookAtMove();
        } else if (currentState == States.DASHING) {

        }
    }

    private void LookAtMove() {
        if (InternalData.moveDirection != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(InternalData.moveDirection);
            _characterController.Move(InternalData.moveDirection);
        }
    }
}