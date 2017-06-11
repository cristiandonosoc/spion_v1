using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : CustomMonoBehaviour {

    [StateMachineEnum]
    public enum States {
        NORMAL = 0,
        DASHING = 1,
        ATTACKING = 2
    }

    #region DATA

    [Serializable]
    public class MoveData {
        public float speed = 0.1f;
    }

    [Serializable]
    public class DashingData {
        public float speed = 0.6f;
        public float currentDuration = 0;
        public float duration = 1.0f;
    }

    [Serializable]
    public class AttackingData {
        public float time = 0.2f;
        public SwordBehaviour weapon;
    }

    [Serializable]
    public class Data {
        public MoveData moveData;
        public DashingData dashingData;
        public AttackingData attackingData;

        public Vector3 moveDirection = new Vector3(0, 0, -1);
        public Vector3 lookDirection;
        public float targetDistance = 10;
        public Vector3 targetVector;


        // TODO(Cristian): Do a much better gravity feel
        public float gravitySpeed = 0.1f;

        public HealthComponentBehaviour healthComponent;

        public Data() {
            moveData = new MoveData();
            dashingData = new DashingData();
            attackingData = new AttackingData();
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

    public MoveData InternalMoveData {
        get { return InternalData.moveData; }
    }
    public DashingData InternalDashingData {
        get { return InternalData.dashingData; }
    }

    public AttackingData InternalAttackingData {
        get { return InternalData.attackingData; }
    }

    public HealthComponentBehaviour HealthComponent {
        get { return InternalData.healthComponent; }
        set { InternalData.healthComponent = value; }
    }

    public float TargetDistance {
        get { return InternalData.targetDistance; }
        set { InternalData.targetDistance = value; }
    }
    public float GravitySpeed {
        get { return InternalData.gravitySpeed; }
        set { InternalData.gravitySpeed = value; }
    }

    public Vector3 MoveDirection { get { return InternalData.moveDirection; } }

    public Vector3 TargetPosition {
        get { return transform.position + InternalData.targetVector; }
    }

    #endregion DATA

    private CharacterController _characterController;
    private Camera _playerCamera;
    private StateMachineBehaviour _stateMachine;
    private ParticleSystem _particleSystem;

    private SwordBehaviour _sword;
    public SwordBehaviour Sword {
        get {
            if (_sword == null) {
                _sword = GetComponentInChildren<SwordBehaviour>();
                if (_sword == null) {
                    _sword = Instantiate<SwordBehaviour>(InternalAttackingData.weapon);
                    _sword.transform.parent = transform;
                }
            }
            return _sword;
        }

    }

    protected override void PlayModeAwake() {
        _characterController = GetComponent<CharacterController>();
        _playerCamera = FindObjectOfType<PlayerCameraBehaviour>().GetComponent<Camera>();
        _stateMachine = GetComponent<StateMachineBehaviour>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _particleSystem.Stop();
        // Just to instantiate
        Sword.Parent = this;

        LookAtMove();
    }

    protected override void PlayModeUpdate() {
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
                    _particleSystem.Play();
                }
            }  if (Input.GetButton("X")) {
                if (_stateMachine.IsCurrentState(States.NORMAL)) {
                    _stateMachine.ChangeState(States.ATTACKING);
                    Sword.ReceiveMessage(SwordBehaviour.MessageKind.ATTACK);
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
            InternalData.moveDirection = new Vector3(x, 0, z);
            InternalData.moveDirection.Normalize();
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
            _characterController.Move(InternalData.lookDirection * InternalDashingData.speed);
            InternalDashingData.currentDuration += Time.deltaTime;
            if (InternalDashingData.currentDuration > InternalDashingData.duration) {
                InternalDashingData.currentDuration = 0;
                _stateMachine.ChangeState(States.NORMAL);
                _particleSystem.Stop();
            }
        }
    }

    private void LookAtMove() {
        if (InternalData.moveDirection != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(InternalData.moveDirection);
            _characterController.Move(InternalData.moveDirection * InternalMoveData.speed);
        }
    }

    #region MESSAGE

    [MessageKindMarker]
    public enum MessageKind {
        ATTACK_STOPPED
    }

    public override void ReceiveMessage(Type msgType, int msgValue, object payload = null) {
        if (msgType == typeof(MessageKind)) {
            ProcessMessage((MessageKind)msgValue, payload);
        } else {
            LogError("Received wrong MessageKind: {0}", msgType.FullName);
        }
    }

    public void ProcessMessage(MessageKind msg, object payload) {
        if (msg == MessageKind.ATTACK_STOPPED) {
            _stateMachine.ChangeState(States.NORMAL);
        }
    }

    #endregion MESSAGE
}