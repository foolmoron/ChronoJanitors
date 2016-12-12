using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDimensionText : MonoBehaviour {
	void Start () {
        GetComponent<Text>().text = "Dimension #" + ItemManager.Inst.CurrentSeed;
    }
}
