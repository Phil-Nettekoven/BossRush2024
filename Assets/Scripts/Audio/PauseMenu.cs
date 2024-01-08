using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//call for changing scenes
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField]
    private GameObject _pauseMenu;
    private void Start()
    {
        _gm = GameManager.Instance;
        if (_gm == null)
        {
            Debug.Log("Pause Menu could not find GameManager");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_gm._pauseGame)
        {
            _pauseMenu.SetActive(true);
        } 
        else
        {
            _pauseMenu.SetActive(false);
        }
    }
    public void BacktoMenu()
    {
        _gm._pauseGame = false;
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
        print("Quit");
    }
}
