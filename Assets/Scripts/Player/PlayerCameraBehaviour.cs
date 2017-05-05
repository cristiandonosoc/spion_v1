using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraBehaviour : MonoBehaviour {

    public bool showDebugOverlay = true;
    public float snapLerpDistance = 1;
    public float snapLerpFactor = 0.05f;

    public float cameraDistance = 15;

    PlayerBehaviour _player;
    Camera _camera;

    Vector2 _screenSize;

	// Use this for initialization
	void Start() {
        _player = FindObjectOfType<PlayerBehaviour>();
        if (_player != null) {
            Debug.Log("Player found");
        }
        _camera = GetComponent<Camera>();
        _screenSize = new Vector2(_camera.pixelWidth, _camera.pixelHeight);
	}


	// Update is called once per frame
	void Update() {
        //float move = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.x);
        //Vector3 targetPos = new Vector3(_player.TargetPosition.x,
        //                                transform.position.y,
        //                                _player.TargetPosition.z - move * diff.magnitude);
        //Vector3 diff = transform.position - _player.TargetPosition;


        //_channel.Feed(transform.position.z);

        //UpdateThroughCameraSpace();

        Vector3 targetPos = TrigonometryTargetPos();
        if (Vector3.Distance(transform.position, targetPos) < snapLerpDistance) {
            transform.position = targetPos;
        } else {
            Vector3 newPos = Vector3.Lerp(transform.position, targetPos, snapLerpFactor);
            transform.position = newPos;
        }
    }

    private Vector3 TrigonometryTargetPos() {
        Vector3 targetPos = _player.TargetPosition;
        float xRotation = 90 - transform.rotation.eulerAngles.x;
        float yRotation = transform.rotation.eulerAngles.y;
        Vector3 cameraDist = Vector3.up * cameraDistance;
        Vector3 diff =  VectorHelpers.RotateAroundPivot(cameraDist, Vector3.zero, Quaternion.Euler(-xRotation, 0, 0));
        diff = VectorHelpers.RotateAroundPivot(diff, Vector3.zero, Quaternion.Euler(0, yRotation, 0));
        return targetPos + diff;
    }

    void ProyectedTargetPosition() {
        Vector3 targetPos = _player.TargetPosition;
        Vector3 proyectedTarget = new Vector3(targetPos.x, transform.position.y, targetPos.z);

        if (Vector3.Distance(transform.position, targetPos) < snapLerpDistance) {
            transform.position = targetPos;
        } else {
            Vector3 newPos = Vector3.Lerp(transform.position, proyectedTarget, snapLerpFactor);
            transform.position = newPos;
        }
    }

    void OnGUI() {
        if (!showDebugOverlay) { return; }
        Vector2 half = _screenSize / 2;
        CustomDebug.LineHelpers.DrawCross(half, 20, Color.white);
        // Detect the player on the screen


        Vector3 screenPos = _camera.WorldToScreenPoint(_player.transform.position);
        CustomDebug.LineHelpers.DrawCross(new Vector2(screenPos.x, _screenSize.y - screenPos.y), 
                  20, Color.green);

        Vector3 targetPos = _camera.WorldToScreenPoint(_player.TargetPosition);
        CustomDebug.LineHelpers.DrawCross(new Vector2(targetPos.x, _screenSize.y - targetPos.y), 20, Color.red);

        //DrawLine(new Vector2(_screenSize.x / 3, 0), new Vector2(_screenSize.x / 3, _screenSize.y), Color.cyan, 1f);
        //DrawLine(new Vector2(2 * _screenSize.x / 3, 0), new Vector2(2 * _screenSize.x / 3, _screenSize.y), Color.cyan, 1f);

        //DrawLine(new Vector2(0, _screenSize.y / 3), new Vector2(_screenSize.x, _screenSize.y / 3), Color.cyan, 1f);
        //DrawLine(new Vector2(0, 2 * _screenSize.y / 3), new Vector2(_screenSize.x, 2 * _screenSize.y / 3), Color.cyan, 1f);
    }


}
