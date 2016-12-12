using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : Manager<GameOver> {
    public bool GameOve;
    public GameObject[] EnableOn;
    public GameObject[] DisableWhen;

    void Update() {
        foreach (var o in EnableOn) {
            o.SetActive(GameOve);
        }
        foreach (var o in DisableWhen) {
            o.SetActive(!GameOve);
        }
	}
}
