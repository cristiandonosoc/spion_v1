using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CustomMonoBehaviour {

    [MessageKindMarker]
    public enum MessageKind {
        HIT
    }

    private SparksParticleSystemBehaviour _sparksParticleSystem;
    public SparksParticleSystemBehaviour SparksParticleSystem {
        get {
            return _sparksParticleSystem;
        }
    }

    protected override void PlayModeAwake() {
        _sparksParticleSystem = GetComponentInChildren<SparksParticleSystemBehaviour>();
    }

    public override void ReceiveMessage(Message message) {
        if (message.type == typeof(MessageKind)) {
            MessageKind msg = (MessageKind)message.messageKind;
            
            if (msg == MessageKind.HIT) {
                Log("HIT");
                SparksParticleSystem.Play();
            }
        }
    }
}
