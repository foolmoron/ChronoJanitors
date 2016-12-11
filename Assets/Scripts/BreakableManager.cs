using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableManager : Manager<BreakableManager> {

    public GameObject BreakableContainer;
    public Dictionary<string, Breakable> Breakables = new Dictionary<string, Breakable>();
    
	void Awake() {
        // get all child rigidbodies
	    foreach (var breakable in BreakableContainer.GetComponentsInChildren<Breakable>()) {
            Breakables[breakable.gameObject.name] = breakable;
	    }
    }
	
	public void AddExplosionForce (float explosionForce, Vector3 explosionPosition, float explosionRadius) {
	    foreach (var breakable in Breakables.Values) {
            breakable.Rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
	    }
	}
}
