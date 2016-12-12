using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetChaosText : MonoBehaviour {
    Text t;
    public string Prefix;
    void Start() {
        t = GetComponent<Text>();
    }
	void Update () {
        t.text = Prefix + PointsManager.Inst.Points.ToString("0.0");
    }
}
