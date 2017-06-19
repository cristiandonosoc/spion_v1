using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : EntityMonoBehaviour {

    #region DATA

    [Serializable]
    public class Data {
        public PlayerBehaviour target;
        public ProjectileBehaviour projectilePrefab;
        public ParticleSystemBehaviour sparksParticleSystemPrefab;
        public ParticleSystemBehaviour explosionParticleSystemPrefab;
        public HealthComponentBehaviour healthComponent;

        public float shotInterval = 1f;
        public float currentShotInterval = 0f;
        public float shotSpeed = 5f;
    }
    [SerializeField]
    private Data _data;
    public Data Dataz {
        get {
            if (_data == null) { _data = new Data(); }
            return _data;
        }
    }

    public ProjectileBehaviour ProjectilePrefab {
        get {
            return Dataz.projectilePrefab;
        }
    }

    // TODO(Cristian): Use generic ParticleBehaviour class
    public ParticleSystemBehaviour SparksParticleSystemPrefab {
        get {
            return Dataz.sparksParticleSystemPrefab;
        }
    }

    private ParticleSystemBehaviour _sparksParticleSystem;
    public ParticleSystemBehaviour SparksParticleSystem {
        get {
            if (_sparksParticleSystem == null) {
                _sparksParticleSystem = Instantiate<ParticleSystemBehaviour>(SparksParticleSystemPrefab);
                _sparksParticleSystem.transform.parent = transform;
                _sparksParticleSystem.transform.localPosition = Vector3.zero;
            }
            return _sparksParticleSystem;
        }
    }

    public ParticleSystemBehaviour ExplosionParticleSystemPrefab {
        get {
            return Dataz.explosionParticleSystemPrefab;
        }
    }

    public HealthComponentBehaviour HealthComponent {
        get {
            return Dataz.healthComponent;
        }
    }

    #endregion DATA

    #region UPDATE

    protected override void PlayModeUpdate() {
        if (Dataz.target != null) {
            Dataz.currentShotInterval += Time.deltaTime;
            if (Dataz.currentShotInterval >= Dataz.shotInterval) {
                Dataz.currentShotInterval -= Dataz.shotInterval;

                Log("Shot");

                var projectile = Instantiate<ProjectileBehaviour>(ProjectilePrefab);
                projectile.Direction = Dataz.target.transform.position - transform.position;
                projectile.Speed = Dataz.shotSpeed;
                projectile.transform.position = transform.position;
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
        } else if (msgType == typeof(HealthComponentBehaviour.Messages)) {
            ProcessMessage((HealthComponentBehaviour.Messages)msgValue, payload);
        } else {
            LogError("Received wrong MessageKind: {0}", msgType.FullName);
        }
    }

    private void ProcessMessage(MessageKind msg, object payload = null) {
        if (msg == MessageKind.HIT) {
            Log("HIT");
            SparksParticleSystem.Play();

            if (HealthComponent != null) {
                HealthComponent.CurrentHP -= 3;
            }
        } else if (msg == MessageKind.ENEMY_ENTER) {
            Collider collider = (Collider)payload;
            PlayerBehaviour player = collider.GetComponent<PlayerBehaviour>();
            if (player == null) { return; }
            if (!player.HasTags(Tag.PLAYER)) { return; }

            Dataz.target = player;
            Log("Player entered");

        } else if (msg == MessageKind.ENEMY_EXIT) {
            Collider collider = (Collider)payload;
            PlayerBehaviour player = collider.GetComponent<PlayerBehaviour>();
            if (player == null) { return; }

            if (player.GUID == Dataz.target.GUID) {
                Dataz.target = null;
                Dataz.currentShotInterval = 0f;

                Log("Player left");
            }
        }
    }

    private void ProcessMessage(HealthComponentBehaviour.Messages msg, object payload = null) {
        if (msg == HealthComponentBehaviour.Messages.NO_HEALTH) {
            var explosion = Instantiate<ParticleSystemBehaviour>(ExplosionParticleSystemPrefab);
            explosion.transform.position = transform.position;
            explosion.Play();
            Destroy(gameObject);
            // We destroy the explostion after a while
            Destroy(explosion.gameObject, explosion.Duration);
        }
    }

    #endregion MESSAGES
}
