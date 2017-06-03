using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HealthBarBehaviour : CustomMonoBehaviour {
    public HealthComponentBehaviour healthComponent;
    public CanvasRenderer healthBar;


    protected override void PlayModeUpdate() {
        if ((healthComponent == null) || (healthBar == null)) { return; }
        InternalUpdate();
	}

    void InternalUpdate() {
        // We see how much we have to move the life
        float ratio = (float)healthComponent.CurrentHP / (float)healthComponent.MaxHP;
        healthBar.transform.localScale = new Vector3(ratio,
                                                      healthBar.transform.localScale.y,
                                                      healthBar.transform.localScale.z);
    }
}
