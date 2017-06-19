using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemBehaviour : CustomMonoBehaviour {

    private ParticleSystem[] _particles;

    public float Duration {
        get {
            float duration = 0;
            foreach (ParticleSystem particleSystem in _particles) {
                if (particleSystem.main.duration > duration) {
                    duration = particleSystem.main.duration;
                }
            }
            return duration;
        }
    }

    protected override void PlayModeAwake() {
        _particles = GetComponents<ParticleSystem>();
    }

    public virtual void Play() {
        foreach (ParticleSystem particleSystem in _particles) {
            particleSystem.Play();
        }
    }

    public virtual void Stop() {
        foreach (ParticleSystem particleSystem in _particles) {
            particleSystem.Stop();
        }
    }
}
