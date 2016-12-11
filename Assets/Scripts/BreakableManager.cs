using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableManager : Manager<BreakableManager> {

    public GameObject BreakableContainer;
    public Rigidbody[] Rigibodies;
    
	void Awake() {
        // get all child rigidbodies
        Rigibodies = BreakableContainer.GetComponentsInChildren<Rigidbody>();
    }
	
	public void AddExplosionForce (float explosionForce, Vector3 explosionPosition, float explosionRadius) {
	    foreach (var rigibody in Rigibodies) {
	        rigibody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
	    }
	}
}
