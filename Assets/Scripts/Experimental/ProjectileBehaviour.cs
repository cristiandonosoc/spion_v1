using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileHitData {
    public int damage;
}

public class ProjectileBehaviour : CustomMonoBehaviour {

    #region DATA

    [Serializable]
    public class Data {
        public Vector3 direction;  
        public float speed = 0.1f;
        // How many seconds will the projectile be alive
        // TODO(Cristian): Make particles use an object pool
        public float lifetime = 5;
        public float currentLifetime = 0;
        public ProjectileHitData projectileHitData;

        public Data() {
            direction = new Vector3(1, 0, 0);
            projectileHitData = new ProjectileHitData();
            projectileHitData.damage = 1;
            direction.Normalize();
        }
    }
    [SerializeField]
    private Data _data;
    public Data Dataz {
        get {
            if (_data == null) { _data = new Data(); }
            return _data;
        }
    }

    public Vector3 Direction {
        get { return Dataz.direction; }
        set { Dataz.direction = value.normalized; }
    }

    public float Speed {
        get { return Dataz.speed; }
        set { Dataz.speed = value; }
    }

    public float Lifetime {
        get { return Dataz.lifetime; }
        set { Dataz.lifetime = value; }
    }

    public float CurrentLifetime {
        get { return Dataz.currentLifetime; }
        set { Dataz.currentLifetime = value; }
    }

    public ProjectileHitData ProjectileHitData {
        get { return Dataz.projectileHitData; }
    }

    #endregion DATA

    protected override void PlayModeUpdate() {
        transform.Translate(Dataz.direction * Dataz.speed * Time.deltaTime);

        CurrentLifetime += Time.deltaTime;
        if (CurrentLifetime > Lifetime) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider collider) {
        CustomMonoBehaviour target = collider.GetComponent<CustomMonoBehaviour>();
        if (target == null) { return; }

        if (!target.HasTags(Tag.PLAYER)) { return; }

        target.ReceiveMessage(PlayerBehaviour.MessageKind.PROJECTILE_COLLISION,
                              Dataz.projectileHitData);
    }

}
