using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    public bool _pauseGame;
    public int _moveCount = 0;

    [SerializeField] private GameObject _player;

    public void Awake()
    {
        //_player = Instantiate(PlayerStats, Vector3.zero);
        //Application.targetFrameRate = 60;
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!_pauseGame)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            }

        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        _pauseGame = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        _pauseGame = false;
    }

    public void SendSignalMove()
    {
        this.BroadcastMessage("NextMove", null, SendMessageOptions.DontRequireReceiver);
        _moveCount += 1;
    }

}
