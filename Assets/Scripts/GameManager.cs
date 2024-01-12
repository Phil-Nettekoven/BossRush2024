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
    public int _moveCount;
    private bool isMoving = false;
    private float timeToMove = 0.05f;
    private float timeToWait = 0.150f;

    [SerializeField] private GameObject _player;

    public void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            _moveCount = 0;
        }
    private void Update()
    {
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(!_pauseGame)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            }
            if (!isMoving)
            {
                if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.UpArrow))) {
                    StartCoroutine(Move(_player, Vector3.up, 1f));
                }
                if ((Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.LeftArrow)))
                {
                    StartCoroutine(Move(_player, Vector3.left, 1f));
                }
                if ((Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.DownArrow)))
                {
                    StartCoroutine(Move(_player, Vector3.down, 1f));
                }
                if ((Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.RightArrow)))
                {
                    StartCoroutine(Move(_player, Vector3.right, 1f));
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
        BroadcastMessage("NextMove");
    }

    public KeyValuePair<string, int> GenerateKeyPair(string str, int integer)
    {
        return new KeyValuePair<string, int>(str, integer);
    }

    public IEnumerator Move(GameObject gameObject, Vector3 direction, float distance)
    {

        if (gameObject == _player)
        {
            isMoving = true;
            SendSignalMove();
        }

        Vector3 origPos, targetPos;

        bool hitWall = false;
        float elapsedTime = 0;

        origPos = gameObject.transform.position;
        targetPos = origPos + (direction * distance);

        int raycastLength = 1;
        RaycastHit2D hit;
        if (hit = Physics2D.Raycast(origPos + direction, direction, raycastLength/2))
        {
            print(hit.collider.gameObject.tag);
            Debug.DrawRay(origPos, direction, Color.green, 2);
            if (hit.collider.gameObject.tag == "Wall")
            {
                print("inga hunga");
                
                hitWall = true;
            }
        }

        while (elapsedTime < timeToMove)
        {
            if(!hitWall) gameObject.transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if(!hitWall) gameObject.transform.position = targetPos; //this line prevents tiny offsets from adding up after many movements, keeping player on grid

        while (elapsedTime < (timeToMove + timeToWait))
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (gameObject == _player) isMoving = false;
    }

}
