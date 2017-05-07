using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatedModelBehaviour : ModelBehaviour {

    private Animator _animator;
    public Animator Animator {
        get {
            if (_animator == null) {
                _animator = GetComponent<Animator>();
            }
            return _animator;
        }
    }

}
