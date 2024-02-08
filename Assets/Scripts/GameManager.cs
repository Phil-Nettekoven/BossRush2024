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
    [SerializeField] private DangerSmall _dangerSmall;
    public bool _pauseGame;
    public int _moveCount = 0;

    [SerializeField] private GameObject _player;

    public void Start()
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

    public DangerSmall createDangerTile(Vector2 pos, int delay, int duration, int spriteChoice){
        print("Created new tile at "+ DateTime.Now);
        DangerSmall dangerTile = Instantiate(_dangerSmall, pos, Quaternion.identity, this.transform);
        dangerTile.Init(delay, duration, spriteChoice);
        return dangerTile;
    }

}
