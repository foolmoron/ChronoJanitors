using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnContact : MonoBehaviour {

    public float ExplosionForce = 20;
    public float ExplosionRadius = 10;

    void OnCollisionStay(Collision collision) {
        if (collision.contacts.Length > 0) {
            var pos = collision.contacts[0].point;
            pos.Scale(new Vector3(Random.value * 1.5f, Random.value * 1.5f, Random.value * 1.5f));
            GetComponent<Rigidbody>().AddExplosionForce(Random.value * ExplosionForce, pos, ExplosionRadius);
        }
    }
}
