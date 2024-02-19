using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    private Vector3 playerPos;

    private float playerDistance;
    private int postStompDelay = 3;
    const float timeToMove = 0.15f;
    const float timeToWait = 0.05f;

    /*SHOCK TILE VARIABLES*/
    const int _shockTileDelay = 1;
    const int _shockTileDuration = 1;
    const int _generatorDelay = 0;
    const int _generatorMaxTurns = 10;

    const int _shockDmg1 = 100; //damage for first shockwave
    const int _shockDmg2 = 50; //second shockwave etc.
    const int _shockDmg3 = 25;



    private GameManager _gm;
    private GridManager _gridManager;
    private Queue<KeyValuePair<string, int>> _queuedMoves;

    public GameObject Player;

    public GameObject MainCamera;

    public GameObject self;

    private Vector3 stompTarget;

    [SerializeField] private ShockTile _shockTilePrefab;

    [SerializeField] private ShockTileGenerator _shockTileGenPrefab;

    private void Start()
    {
        _gm = GameManager.Instance;
        Player = GameObject.Find("Player");
        MainCamera = GameObject.Find("Main Camera");
        _gridManager = GridManager.Instance;
        _queuedMoves = new Queue<KeyValuePair<string, int>>();

        for (int i = 0; i < 20; i++)
        { //boss does nothing for i turns
            _queuedMoves.Enqueue(GenerateKeyPair("idle", i));
        }
    }

    void Update()
    {

    }

    private void NextMove()
    {
        //print(queuedMoves.Count);
        playerPos = Player.transform.position;
        playerDistance = Vector3.Distance(transform.position, playerPos);


        if (_queuedMoves.Count > 0 && _queuedMoves.Peek().Key == "idle" && playerDistance < 8)
        { //Player has entered fight radius
            _queuedMoves.Clear();
        }
        else if (_queuedMoves.Count > 0) //dequeue current moves
        {
            interpretMove(_queuedMoves.Dequeue());
        }
        else if (postStompDelay <= 0 && playerDistance <= 15) //Add "stomp" attack to queue
        {
            for (int i = 0; i < 15; i++)
            {
                _queuedMoves.Enqueue(GenerateKeyPair("stomp", i));
            }
        }
        else if (postStompDelay <= 0 && playerDistance > 15)
        {
            for (int i = 0; i < 5; i++)
            {
                _queuedMoves.Enqueue(GenerateKeyPair("ranged1", i));
                postStompDelay = 0;
            }
        }
        else
        {
            postStompDelay -= 1;
        }
    }

    private void interpretMove(KeyValuePair<string, int> move)
    {
        //print(move.Key);
        switch (move.Key)
        {
            case "stomp":
                Stomp(move.Value);
                break;
            case "ranged1":
                break;
            default:
                break;
        }
    }

    private void Stomp(int step)
    {
        switch (step)
        {
            case 0: //Fly into air
                Vector3 temp = Player.transform.position;
                temp.z = -20;
                StartCoroutine(JumpUp(temp));
                break;
            case 3: //Telegraph attack (exclamation marker)
                stompTarget = playerPos;
                _gm.createDangerTile(stompTarget, 0, 3, 1); //create large danger tile on target position
                break;
            case 7: //Crash down (damage on impact zone)
                StartCoroutine(JumpDown(stompTarget));
                break;
            case 10: //shockwave 1 (lesser damage to immediate surroundings)
                break;
            case 12: //shockwave 2 (lesser lesser damage to further surroundings)
                break;
            case 14: //end
                postStompDelay = 15;
                break;
            default: //do nothing
                break;
        }
    }

    private IEnumerator JumpUp(Vector3 targetPos)
    {
        float elapsedTime = 0;
        Vector3 origPos = transform.position;
        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //transform.position = stompTarget;
        //yield break;
    }
    private IEnumerator JumpDown(Vector3 targetPos)
    {
        float elapsedTime = 0;
        Vector3 origPos = transform.position;
        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        createShockGenerators(stompTarget);
        //yield break;
    }



    private void createShockGenerators(Vector2 origin)
    {
        ShockTileGenerator temp;

        temp = Instantiate(_shockTileGenPrefab, origin, Quaternion.identity, _gm.transform);
        temp.Init(_generatorDelay, _generatorMaxTurns, _shockTileDelay, _shockTileDuration, 1f, _shockDmg1); //wave1

        temp = Instantiate(_shockTileGenPrefab, origin, Quaternion.identity, _gm.transform);
        temp.Init(_generatorDelay + 2, _generatorMaxTurns, _shockTileDelay, _shockTileDuration, 0.75f, _shockDmg2); //wave2

        temp = Instantiate(_shockTileGenPrefab, origin, Quaternion.identity, _gm.transform);
        temp.Init(_generatorDelay + 6, _generatorMaxTurns, _shockTileDelay, _shockTileDuration, 0.4f, _shockDmg3); //wave2

    }




    public KeyValuePair<string, int> GenerateKeyPair(string str, int integer)
    {
        return new KeyValuePair<string, int>(str, integer);
    }

}


