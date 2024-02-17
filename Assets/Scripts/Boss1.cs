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

    const int _shockTileDelay = 1;

    const int _shockTileDuration = 1;

    private GameManager _gm;
    private GridManager _gridManager;
    private Queue<KeyValuePair<string, int>> _queuedMoves;

    public GameObject Player;

    public GameObject MainCamera;

    public GameObject self;

    private Vector3 stompTarget;
    private List<Vector2> _unstompableTiles;

    private Queue<ShockTile> _shockTileQueue;
    private List<Vector2> _shockTileLocations;

    [SerializeField] private ShockTile _shockTilePrefab;

    private void Start()
    {
        _gm = GameManager.Instance;
        Player = GameObject.Find("Player");
        MainCamera = GameObject.Find("Main Camera");
        _gridManager = GridManager.Instance;
        _queuedMoves = new Queue<KeyValuePair<string, int>>();
        _shockTileQueue = new Queue<ShockTile>();
        _unstompableTiles = new List<Vector2>();
        _shockTileLocations = new List<Vector2>();
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
        if (_shockTileQueue.Count > 0)
        {
            shockWaveSearch();
        }

        else if (_queuedMoves.Count > 0 && _queuedMoves.Peek().Key == "idle" && playerDistance < 8)
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
        createShockWave(stompTarget);
        //yield break;
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

    private void createShockWave(Vector2 origin)
    {
        //_stompableTile.Add(origin);
        Vector2 targetPos;
        ShockTile spawnedTile;
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++) //find the 3x3 tile grid where the boss crashed down
            {
                _unstompableTiles.Add(origin + new Vector2(x, y));

                if (y == 1) //create shock tiles above boss
                {
                    //print("y == 1");
                    //print(origin + new Vector2(x, y + 1));
                    targetPos = origin + new Vector2(x, y + 1);
                    spawnedTile = createShockTile(targetPos, "up");
                    if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); spawnedTile = null; }

                    if (x == 1) //create top right corner tiles
                    {
                        targetPos = origin + new Vector2(x + 1, y + 1);
                        spawnedTile = createShockTile(targetPos, "up");
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); spawnedTile = null; }
                        spawnedTile = createShockTile(targetPos, "right", true);//force instantiate second corner tile
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); spawnedTile = null; }
                    }
                    else if (x == -1) //top left corner tiles
                    {
                        targetPos = origin + new Vector2(x - 1, y + 1);
                        spawnedTile = createShockTile(targetPos, "up");
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); spawnedTile = null; }
                        spawnedTile = createShockTile(targetPos, "left", true); //force instantiate second corner tile
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); spawnedTile = null; }
                    }
                }
                else if (y == -1) //create shock tiles under boss
                {
                    //print("y == -1");
                    targetPos = origin + new Vector2(x, y - 1);
                    spawnedTile = createShockTile(targetPos, "down");
                    if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); spawnedTile = null; }

                    if (x == 1) //create bottom right corner tiles
                    {
                        targetPos = origin + new Vector2(x + 1, y - 1);
                        spawnedTile = createShockTile(targetPos, "down");
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); spawnedTile = null; }
                        spawnedTile = createShockTile(targetPos, "right", true); //force instantiate second corner tile
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); spawnedTile = null; }

                    }
                    else if (x == -1) //create bottom left corner tiles
                    {
                        targetPos = origin + new Vector2(x - 1, y - 1);
                        spawnedTile = createShockTile(targetPos, "down");
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); spawnedTile = null; }
                        spawnedTile = createShockTile(targetPos, "left", true); //force instantiate second corner tile
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); spawnedTile = null; }
                    }
                }


                if (x == -1)//create shock tiles left of boss
                {
                    //print("x == -1");
                    targetPos = origin + new Vector2(x - 1, y);
                    spawnedTile = createShockTile(targetPos, "left");
                    if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); spawnedTile = null; }
                }
                else if (x == 1)//create shock tiles right of boss
                {
                    //print("x == 1");
                    targetPos = origin + new Vector2(x + 1, y);
                    spawnedTile = createShockTile(targetPos, "right");
                    if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); spawnedTile = null; }
                }
            }
        }

    }

    private void shockWaveSearch(bool random = true, int maxDistance = 1)
    {
        Queue<ShockTile> newQueue = new Queue<ShockTile>();
        ShockTile curTile;
        Vector2 curTilePos;
        Vector2 randomModifier = Vector2.zero;
        Vector2 scatter = Vector2.zero;
        
        while (_shockTileQueue.Count > 0)
        {
            curTile = _shockTileQueue.Dequeue();
            curTilePos = curTile.transform.position;

            for (int i = -1; i < 2; i++)
            {
                scatter = Vector2.zero;
                ShockTile spawnedTile = null;
                if (random) { randomModifier = generateRandomModifier(maxDistance, curTile.getDirection()); }

                switch (curTile.getDirection())
                {
                    case "up":
                        scatter.x += i;
                        spawnedTile = createShockTile(curTilePos + Vector2.up + randomModifier, "up");
                        break;
                    case "down":
                        scatter.x += i;
                        spawnedTile = createShockTile(curTilePos + Vector2.down + randomModifier, "down");
                        break;
                    case "left":
                        scatter.y += i;
                        spawnedTile = createShockTile(curTilePos + Vector2.left + randomModifier, "left");
                        break;
                    case "right":
                        scatter.y += i;
                        spawnedTile = createShockTile(curTilePos + Vector2.right + randomModifier, "right");
                        break;
                    default:
                        break;
                }
                if (spawnedTile) { newQueue.Enqueue(spawnedTile); }
            }
            _shockTileLocations.Remove(curTilePos);
        }
        if (newQueue.Count == 0)
        {
            _unstompableTiles.Clear();
        }
        else
        {
            _shockTileQueue = newQueue;
        }
        _shockTileLocations.Clear();

    }

    private ShockTile createShockTile(Vector2 targetPos, string direction, bool forceInstantiate = false)
    {
        ShockTile spawnedTile = null;

        if (!forceInstantiate)
        {
            if (_gridManager.isInsideGrid(targetPos) && !_unstompableTiles.Contains(targetPos) && !_shockTileLocations.Contains(targetPos) && _gridManager.GetTileAtPosition(targetPos) != "Wall")
            {
                _shockTileLocations.Add(targetPos);
                _unstompableTiles.Add(targetPos);
                spawnedTile = Instantiate(_shockTilePrefab, targetPos, Quaternion.identity, _gm.transform);
                spawnedTile.Init(_shockTileDelay, _shockTileDuration, direction);
            }
        }
        else
        { //Force instantiate ignores some instantiation requirements (mostly for corner pieces)
            if (_gridManager.isInsideGrid(targetPos) && _gridManager.GetTileAtPosition(targetPos) != "Wall")
            {
                spawnedTile = Instantiate(_shockTilePrefab, targetPos, Quaternion.identity, _gm.transform);
                spawnedTile.Init(_shockTileDelay, _shockTileDuration, direction);
            }

        }

        if (spawnedTile) { _gm.createDangerTile(targetPos, 0, 0, 0); }

        return spawnedTile;
    }
    private Vector2 generateRandomModifier(int maxDistance, string direction)
    {
        Vector2 randomModifier = Vector2.zero;
        if (direction == "up" || direction == "down")
        {
            randomModifier.x = UnityEngine.Random.Range(-1, 1);
            randomModifier.y = UnityEngine.Random.Range(maxDistance * -1,  maxDistance);
        }
        else
        {
            randomModifier.x = UnityEngine.Random.Range(maxDistance * -1, maxDistance);
            randomModifier.y = UnityEngine.Random.Range(-1, 1);

        }

        return randomModifier;


    }

    private string randomDirection(int location = -1) //return a random direction for corner tiles to travel
    {

        //directions[0] = top left corner
        //directions[1] = top right corner
        //directions[2] = bottom left corner
        //directions[3] = bottom right corner
        string[,] directions = { { "up", "left" }, { "up", "right" }, { "down", "left" }, { "down", "right" } };
        if (location == -1)
        {
            return directions[UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 1)];
        }
        return directions[location, UnityEngine.Random.Range(0, 1)];
    }

    public KeyValuePair<string, int> GenerateKeyPair(string str, int integer)
    {
        return new KeyValuePair<string, int>(str, integer);
    }

}


