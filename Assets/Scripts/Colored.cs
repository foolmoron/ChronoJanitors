using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colored : MonoBehaviour {

    public Color Color = Color.white;
    public bool RandomColor;

    new Renderer renderer;
    
	void Start () {
        // random color
        if (RandomColor) {
            Color = new HSBColor(Random.value, 0.6f, 1f).ToColor();
        }
        // copy material
        renderer = GetComponent<Renderer>();
        renderer.material = new Material(renderer.sharedMaterial);
	}
	
	void Update () {
	    renderer.material.SetColor("Main Color", Color);
        renderer.material.color = Color;
	}
}
