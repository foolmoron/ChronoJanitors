using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaos : MonoBehaviour {

    Rigidbody rb;

    void HandleCollision(Collision collision) {
        if (!rb) {
            rb = GetComponent<Rigidbody>();
        }
        if (!rb) {
            return;
        }
        var force = 0f;
        foreach (var contact in collision.contacts) {
            force = Mathf.Max(force, Vector3.Dot(contact.normal, collision.relativeVelocity) * rb.mass * (contact.otherCollider.attachedRigidbody ? contact.otherCollider.attachedRigidbody.mass : 1));
        }
        PointsManager.Inst.AddChaos(force);
    }
    
	void OnCollisionEnter(Collision collision) {
        HandleCollision(collision);
    }
    
	void OnCollisionStay(Collision collision) {
        HandleCollision(collision);
	}
}
