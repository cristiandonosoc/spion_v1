using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparksParticleSystemBehaviour : MonoBehaviour {

    ParticleSystem _particleSystem;
    ParticleSystem ParticleSystemz {
        get {
            if (_particleSystem == null) {
                _particleSystem = GetComponent<ParticleSystem>();
            }
            return _particleSystem;
        }
    }
    ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[25];

	// Use this for initialization
	void Awake() {
        // We initialize
        _particleSystem = ParticleSystemz;
	}


    public void Play() {
        ParticleSystemz.Play();

        StartCoroutine(LateFix());
    }

    IEnumerator LateFix() {
        yield return new WaitForEndOfFrame();
        FixParticleRotations();
    }

    private void FixParticleRotations() {
        ParticleSystemz.GetParticles(_particles);

        for (int i = 0; i < _particles.Length; i++) {
            ParticleSystem.Particle particle = _particles[i];
            Vector3 dir = particle.velocity.normalized;
            float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _particles[i].rotation = rot;
        }

        ParticleSystemz.SetParticles(_particles, ParticleSystemz.particleCount);
    }
}
