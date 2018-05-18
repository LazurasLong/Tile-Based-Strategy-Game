using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceMan : MonoBehaviour {

    int CurScene = 0;
    public void NextScence()
    {
        CurScene++;
        SceneManager.LoadScene(CurScene);
    }

    public void ReturnScene()
    {
        SceneManager.LoadScene(CurScene);
    }

    public void StartOptions()
    {
        SceneManager.LoadScene("StartOptions");
        //SceneManager.LoadScene(3);
    }

    public void MainMenu()
    {
        CurScene = 0;
        SceneManager.LoadScene(CurScene);
    }
}
