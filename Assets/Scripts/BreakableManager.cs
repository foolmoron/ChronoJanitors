using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableManager : Manager<BreakableManager> {

    public GameObject BreakableContainer;
    public Dictionary<string, Breakable> Breakables = new Dictionary<string, Breakable>();
    
	void Awake() {
    }

    public void CacheBreakables() {
        foreach (var breakable in BreakableContainer.GetComponentsInChildren<Breakable>()) {
            Breakables[breakable.gameObject.name] = breakable;
            breakable.Init();
        }
    }

    static void AddExp(Rigidbody rb, float force, Vector3 pos) {
        var dir = rb.centerOfMass - pos;
        rb.AddForce(dir.normalized * force);
        rb.AddTorque(Random.value * force, Random.value * force, Random.value * force);
    }
	
	public void AddExplosionForce (float explosionForce, Vector3 explosionPosition) {
	    foreach (var breakable in Breakables.Values) {
            Debug.Log("Exploding " + breakable.name + " - " + explosionForce + explosionPosition);
            AddExp(breakable.Rigidbody, explosionForce, explosionPosition);
	        foreach (var rb in breakable.Rigidbodies) {
	            if (rb != null) {
	                AddExp(rb, explosionForce, explosionPosition);
                }
	        }
	    }
	}
}
