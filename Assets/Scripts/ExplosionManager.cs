using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : Manager<ExplosionManager> {

    public ParticleSystem Cursor;
    public GameObject Explosion;

    RaycastHit hit;

    void Update() {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out hit)) {
            Cursor.enableEmission(true);
            Cursor.transform.position = hit.point;
            if (Input.GetMouseButtonDown(0)) {
                Instantiate(Explosion, hit.point, Quaternion.identity);
            }
        } else {
            Cursor.enableEmission(false);
        }
    }
}
