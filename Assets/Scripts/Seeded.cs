using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Seeded : MonoBehaviour {
    public InputField Field;
	public void Clicked() {
        int i;
        ItemManager.LevelGenerationSeed = int.TryParse(Field.text, out i) ? i : Field.text.GetHashCode();
        SceneManager.LoadScene("main");
    }
}
