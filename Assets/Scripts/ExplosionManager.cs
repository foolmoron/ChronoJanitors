using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : Manager<ExplosionManager> {

    public ParticleSystem Cursor;
    public GameObject Explosion;

    public List<float> ExplosionTimes = new List<float>(10);

    RaycastHit hit;

    void Update() {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out hit)) {
            Cursor.enableEmission(true);
            Cursor.transform.position = hit.point;
            if (Input.GetMouseButtonDown(0)) {
                Instantiate(Explosion, hit.point, Quaternion.identity);
                ExplosionTimes.Add(TimeManager.Inst.VirtualTime);
            }
        } else {
            Cursor.enableEmission(false);
        }
        for (int i = 0; i < ExplosionTimes.Count; i++) {
            if (ExplosionTimes[i] > TimeManager.Inst.VirtualTime) {
                ExplosionTimes.RemoveAt(i);
                i--;
            }
        }
    }
}
