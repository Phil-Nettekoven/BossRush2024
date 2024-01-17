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
    const float timeToMove = 0.15f;
    const float timeToWait = 0.05f;
    private float playerMoveDistance = 1f;
    const int rollCoolDown = 5;
    private int rollTimer;

    private List<MonoBehaviour> monoList = new List<MonoBehaviour>();


    void BroadcastMessageExt(string methodName)
    {
        GetComponentsInChildren<MonoBehaviour>(true, monoList);
        for (int i = 0; i < monoList.Count; i++)
        {
            monoList[i].Invoke(methodName, 0);
        }
    }


    [SerializeField] private GameObject _player;

    public void Awake()
    {
        QualitySettings.vSyncCount = 120;
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
            if (!isMoving)
            {
                if (Input.GetKey(KeyCode.Space) && rollTimer <= 0)
                {
                    playerMoveDistance = 2f;
                }
                else
                {
                    playerMoveDistance = 1f;
                }
                if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.UpArrow)))
                {
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
        this.BroadcastMessage("NextMove", null, SendMessageOptions.DontRequireReceiver);
        _moveCount += 1;
    }

    public IEnumerator Move(GameObject gameObject, Vector3 direction, float distance)
    {
        Vector3 origPos, targetPos;
        bool hitWall = false;
        float elapsedTime = 0;

        int raycastLength = 1;

        Quaternion origRot, targetRot;

        if (gameObject == _player && rollTimer > 0)
        {
            distance = 1f;
            rollTimer -= 1;
        }

        int divisor = (distance == 2f) ? divisor = 1 : divisor = 2;

        origPos = gameObject.transform.position;
        targetPos = origPos + (direction * distance);

        origRot = gameObject.transform.rotation;
        targetRot = origRot * Quaternion.Euler(0, 0, 360);

        //Debug.DrawRay(origPos + direction, direction, Color.green, 2);
        RaycastHit2D hit;

        if (hit = Physics2D.Raycast(origPos + direction, direction, raycastLength / divisor))
        {
            print(hit.collider.gameObject.tag);
            if (hit.collider.gameObject.tag == "Wall")
            {
                if (Vector3.Distance(gameObject.transform.position, hit.collider.gameObject.transform.position) > 1)
                { //check if distance > 1 unit, try next closest tile.
                    StartCoroutine(Move(gameObject, direction, distance - 1));
                    yield break;
                }
                hitWall = true;
            }
        }

        if (gameObject == _player)
        {
            if (distance > 1f && rollTimer <= 0)
            {
                rollTimer = rollCoolDown;
            }
            isMoving = true;
            SendSignalMove();
        }

        while (elapsedTime < timeToMove)
        {
            if (!hitWall) gameObject.transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            if (distance == 2f && gameObject == _player) //spiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiin
            {
                float rollDegrees;
                if (direction == Vector3.left || direction == Vector3.down) rollDegrees = 360f;
                else rollDegrees = -360f;
                float rot = Mathf.Lerp(0, rollDegrees, (elapsedTime / timeToMove + timeToWait));
                gameObject.transform.rotation = Quaternion.Euler(0, 0, rot);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!hitWall) gameObject.transform.position = targetPos; //this line prevents tiny offsets from adding up after many movements, keeping player on grid
        if (distance == 2) gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        while (elapsedTime < (timeToMove + timeToWait))
        {

            //this block doesn't have to be here but it makes spiiiiiiiiiiin look a bit better in exchange for terrible optimization (gives extra time to spiiin)
            //DON'T FORGET if we decide to remove it to delete "+ timeToWait" in twin block above
            if (distance == 2f && gameObject == _player)
            {
                float rollDegrees;
                if (direction == Vector3.left || direction == Vector3.down) rollDegrees = 360f;
                else rollDegrees = -360f;
                float rot = Mathf.Lerp(0, rollDegrees, (elapsedTime / timeToMove + timeToWait));
                gameObject.transform.rotation = Quaternion.Euler(0, 0, rot);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (gameObject == _player) isMoving = false;
    }

}
