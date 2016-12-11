using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableManager : Manager<BreakableManager> {

    public Rigidbody[] Rigibodies;
    
	void Start () {
        // get all child rigidbodies
        Rigibodies = GetComponentsInChildren<Rigidbody>();

    }
	
	public void AddExplosionForce (float explosionForce, Vector3 explosionPosition, float explosionRadius) {
	    foreach (var rigibody in Rigibodies) {
	        rigibody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
	    }
	}
}
