using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class New : MonoBehaviour
{
    public void Clicked()
    {
        ItemManager.LevelGenerationSeed = 0;
        SceneManager.LoadScene("main");
    }
}
