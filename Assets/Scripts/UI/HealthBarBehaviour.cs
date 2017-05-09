using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HealthBarBehaviour : MonoBehaviour {

    // TODO(Cristian): This should be a generic life holder
    public PlayerBehaviour _player;

    private CanvasRenderer _healthBar;

    void Awake() {
        _player = FindObjectOfType<PlayerBehaviour>();
        _healthBar = transform.FindChild("HealthBar").GetComponent<CanvasRenderer>();
    }


	// Update is called once per frame
	void Update() {
        // We see how much we have to move the life
        float ratio = (float)_player.currentHP / (float)_player.maxHP;
        _healthBar.transform.localScale = new Vector3(ratio,
                                                      _healthBar.transform.localScale.y,
                                                      _healthBar.transform.localScale.z);
	}
}
