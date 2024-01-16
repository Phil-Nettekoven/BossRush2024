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
    private bool isMoving = false;
    const float timeToMove = 0.05f;
    const float timeToWait = 0.150f;
    private float playerMoveDistance = 1f;
    const int rollCoolDown = 5;
    private int rollTimer;


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
                if (Input.GetKey(KeyCode.Space) && rollTimer <= 0){
                    playerMoveDistance = 2f;
                } else{
                    playerMoveDistance = 1f;
                }
                if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.UpArrow))) {
                    StartCoroutine(Move(_player, Vector3.up, playerMoveDistance));
                }
                if ((Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.LeftArrow)))
                {
                    StartCoroutine(Move(_player, Vector3.left, playerMoveDistance));
                }
                if ((Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.DownArrow)))
                {
                    StartCoroutine(Move(_player, Vector3.down, playerMoveDistance));
                }
                if ((Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.RightArrow)))
                {
                    StartCoroutine(Move(_player, Vector3.right, playerMoveDistance));
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
        _moveCount += 1;
    }

    public KeyValuePair<string, int> GenerateKeyPair(string str, int integer)
    {
        return new KeyValuePair<string, int>(str, integer);
    }

    public IEnumerator Move(GameObject gameObject, Vector3 direction, float distance)
    {


        Vector3 origPos, targetPos;
        bool hitWall = false;
        float elapsedTime = 0;
        
        int raycastLength = 1;

        if (gameObject == _player && rollTimer > 0){
            distance = 1f;
            rollTimer -= 1;
        }

        int divisor = (distance == 2f) ? divisor = 1 : divisor = 2;

        origPos = gameObject.transform.position;
        targetPos = origPos + (direction * distance);

        //Debug.DrawRay(origPos + direction, direction, Color.green, 2);
        RaycastHit2D hit;
        
        if (hit = Physics2D.Raycast(origPos + direction, direction, raycastLength/divisor))
        {
            print(hit.collider.gameObject.tag);
            if (hit.collider.gameObject.tag == "Wall")
            {
                if (Vector3.Distance(gameObject.transform.position, hit.collider.gameObject.transform.position) > 1){ //check if distance > 1 unit, try next closest tile.
                    StartCoroutine(Move(gameObject, direction, distance - 1));
                    yield break;
                }
                print("inga hunga");
                
                hitWall = true;
            }
        }

        if (gameObject == _player)
        {
            if (distance > 1f && rollTimer <= 0){
                rollTimer = rollCoolDown;
            }
            isMoving = true;
            SendSignalMove();
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
