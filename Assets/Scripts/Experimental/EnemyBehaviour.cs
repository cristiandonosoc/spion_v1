﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CustomMonoBehaviour {

    [MessageKindMarker]
    public enum MessageKind {
        HIT
    }

    [MessageKindMarker]
    public enum TriggerMessages {
        ENEMY_ENTER
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

    public override void ReceiveMessage(Message message) {
        if (message.type == typeof(MessageKind)) {
            MessageKind msg = (MessageKind)message.messageKind;
            
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
            }
        } else if (message.type == typeof(TriggerMessages)) {
            TriggerMessages msg = (TriggerMessages)message.messageKind;

        }
    }

    public override void ReceiveMessage<T>(T msg, object payload = null) {
        if (typeof(T) == typeof(MessageKind)) {
            ReceiveMessage((MessageKind)Convert.ChangeType(msg, typeof(MessageKind)), payload);
        }
    }

    private void ReceiveMessage(MessageKind msg, object payload = null) {
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
        }
    }
}