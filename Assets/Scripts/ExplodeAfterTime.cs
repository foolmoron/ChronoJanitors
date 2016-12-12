using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeAfterTime : MonoBehaviour {

    public ParticleSystem ps;
    public float CreationTime;
    public float Time = 0.25f;
    float t;


    [Range(0, 100)]
    public float Force = 20;

    new Renderer renderer;
    
	void Start () {
	}
	
	void FixedUpdate () {
	    if (TimeManager.Inst.VirtualTime >= CreationTime + Time) {
	        // explode
            BreakableManager.Inst.AddExplosionForce(Force, transform.position);
            Destroy(gameObject);
            AudioManager.Inst.Play("explosion");
        } else if (TimeManager.Inst.VirtualTime < CreationTime) {
	        Destroy(gameObject);
	    }
	}
}
