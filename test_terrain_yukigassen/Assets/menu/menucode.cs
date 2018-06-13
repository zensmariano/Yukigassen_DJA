using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class menucode : MonoBehaviour {


    public void SairJogo()
    {
        Application.Quit();
    }

    public void play()
    {
        SceneManager.LoadScene("terraintest1yukigassen");
    }
}
