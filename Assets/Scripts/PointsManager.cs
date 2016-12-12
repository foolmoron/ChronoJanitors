using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : Manager<PointsManager> {

    public float Points;

    void Update() {

    }

    public void AddChaos(float chaos) {
        Points += chaos;
    }
}
