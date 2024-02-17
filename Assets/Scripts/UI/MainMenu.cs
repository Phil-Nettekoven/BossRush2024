using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//call for changing scenes
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    public void QuitGame()
    {
        Application.Quit();
        print("Quit");
    }
}

