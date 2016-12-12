using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysParticles : MonoBehaviour {

    ParticleSystem p;

    void Start() {
        p = GetComponent<ParticleSystem>();
    }
    
    void Update() {
        p.Simulate(Time.fixedDeltaTime, true, false);
    }
}
