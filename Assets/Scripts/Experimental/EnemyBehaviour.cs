using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CustomMonoBehaviour {

    [Serializable]
    public class Data {
        public PlayerBehaviour target;

        public float shotInterval = 1f;
        public float currentShotInterval = 0f;
    }
    [SerializeField]
    private Data _data;
    public Data Dataz {
        get {
            if (_data == null) {
                _data = new Data();
            }
            return _data;
        }
    }


    [SerializeField]
    private SparksParticleSystemBehaviour _sparksParticleSystem;
    public SparksParticleSystemBehaviour SparksParticleSystem {
        get {
            if (_sparksParticleSystem == null) {
                _sparksParticleSystem = GetComponentInChildren<SparksParticleSystemBehaviour>();
            }
            return _sparksParticleSystem;
        }
    }

    [SerializeField]
    private ParticleSystem _explosionParticleSystemPrefab;
    public ParticleSystem ExplosionParticleSystemPrefab {
        get {
            return _explosionParticleSystemPrefab;
        }
    }

    private HealthComponentBehaviour _healthComponent;

    protected override void PlayModeAwake() {
        _sparksParticleSystem = GetComponentInChildren<SparksParticleSystemBehaviour>();
        _healthComponent = GetComponent<HealthComponentBehaviour>();
    }

    #region UPDATE

    protected override void PlayModeUpdate() {
        if (Dataz.target != null) {
            Dataz.currentShotInterval += Time.deltaTime;
            if (Dataz.currentShotInterval >= Dataz.shotInterval) {
                Dataz.currentShotInterval -= Dataz.shotInterval;

                Log("Shot");
            }
        }
    }

    #endregion UPDATE

    #region MESSAGES

    [MessageKindMarker]
    public enum MessageKind {
        HIT,
        ENEMY_ENTER,
        ENEMY_EXIT
    }

    public override void ReceiveMessage(Type msgType, int msgValue, object payload = null) {
        if (msgType == typeof(MessageKind)) {
            ProcessMessage((MessageKind)msgValue, payload);
        } else {
            LogError("Received wrong MessageKind: {0}", msgType.FullName);
        }
    }

    private void ProcessMessage(MessageKind msg, object payload = null) {
        if (msg == MessageKind.HIT) {
            Log("HIT");
            SparksParticleSystem.Play();

            if (_healthComponent != null) {
                _healthComponent.CurrentHP -= 3;

                if (_healthComponent.CurrentHP == 0) {
                    if (ExplosionParticleSystemPrefab != null) {
                        var explosion = Instantiate<ParticleSystem>(ExplosionParticleSystemPrefab);
                        explosion.transform.position = transform.position;
                        explosion.Play();
                        Destroy(gameObject);
                        // We destroy the explostion after a while
                        Destroy(explosion.gameObject, explosion.main.duration);
                    }
                }
            }
        } else if (msg == MessageKind.ENEMY_ENTER) {
            Collider collider = (Collider)payload;
            if (collider.tag != "Player") { return; }
            PlayerBehaviour player = collider.GetComponent<PlayerBehaviour>();
            if (player == null) { return; }

            Dataz.target = player;
            Log("Player entered");

        } else if (msg == MessageKind.ENEMY_EXIT) {
            Collider collider = (Collider)payload;
            // TODO(Cristian): Use a unique ID
            if (collider.tag == "Player") { 
                Dataz.target = null;
                Dataz.currentShotInterval = 0f;

                Log("Player left");
            }
        }
    }

    #endregion MESSAGES
}
