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

    const int _shockTileDelay = 0;

    const int _shockTileDuration = 0;

    private GameManager _gm;
    private GridManager _gridManager;
    private Queue<KeyValuePair<string, int>> _queuedMoves;

    public GameObject Player;

    public GameObject MainCamera;

    public GameObject self;

    private Vector3 stompTarget;
    private Dictionary<Vector2, bool> _stompableTile = new Dictionary<Vector2, bool>();

    private Queue<KeyValuePair<ShockTile, string>> _shockTileQueue;

    [SerializeField] private ShockTile _shockTile;

    private void Start()
    {
        _gm = GameManager.Instance;
        Player = GameObject.Find("Player");
        MainCamera = GameObject.Find("Main Camera");
        _gridManager = GridManager.Instance;
        _queuedMoves = new Queue<KeyValuePair<string, int>>();
        _shockTileQueue = new Queue<KeyValuePair<ShockTile, string>>();
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
                _gm.createDangerTile(stompTarget, 0, 3, 1); //create large danger tile on player position
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
        //_stompableTile[origin] = false;
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++) //find the 3x3 tile grid where the boss crashed down
            {
                Vector2 shockTileLoc = origin;
                string shockTileDir = null;
                ShockTile spawnedTile = null;
                if (y == 1) //create shock tiles above boss
                {
                    print(origin + new Vector2(x, y + 1));
                    shockTileLoc = origin + new Vector2(x, y + 1);
                    shockTileDir = "up";
                }
                if (y == -1) //create shock tiles under boss
                {
                    shockTileLoc = origin + new Vector2(x, y - 1);
                    shockTileDir = "down";
                }
                if (x == -1)//create shock tiles left of boss
                {
                    shockTileLoc = origin + new Vector2(x - 1, y);
                    shockTileDir = "left";
                }
                if (x == 1)//create shock tiles right of boss
                {
                    shockTileLoc = origin + new Vector2(x + 1, y);
                    shockTileDir = "right";
                }
                
                spawnedTile = createShockTile(shockTileLoc);

                if (spawnedTile != null)
                {
                    _shockTileQueue.Enqueue(new KeyValuePair<ShockTile, string>(spawnedTile, shockTileDir));
                    //_stompableTile[shockTileLoc] = false;
                }

                _stompableTile[origin + new Vector2(x, y)] = false;

                //print(origin + new Vector2(x,y));
            }
        }

    }

    private void shockWaveSearch()
    {
        Queue<KeyValuePair<ShockTile, string>> newQueue = new Queue<KeyValuePair<ShockTile, string>>();
        while (_shockTileQueue.Count > 0)
        {
            KeyValuePair<ShockTile, string> curTile = _shockTileQueue.Dequeue();
            Vector2 curTilePos = curTile.Key.transform.position;
            ShockTile spawnedTile;
            switch (curTile.Value)
            {
                case "up":
                    spawnedTile = createShockTile(curTilePos + Vector2.up);
                    if (spawnedTile) { newQueue.Enqueue(new KeyValuePair<ShockTile, string>(spawnedTile, "up")); }

                    break;
                case "down":
                    spawnedTile = createShockTile(curTilePos + Vector2.down);
                    if (spawnedTile) { newQueue.Enqueue(new KeyValuePair<ShockTile, string>(spawnedTile, "down")); }
                    break;
                case "left":
                    spawnedTile = createShockTile(curTilePos + Vector2.left);

                    if (spawnedTile != null) { newQueue.Enqueue(new KeyValuePair<ShockTile, string>(spawnedTile, "left")); }
                    break;
                case "right":
                    spawnedTile = createShockTile(curTilePos + Vector2.right);
                    if (spawnedTile != null) { newQueue.Enqueue(new KeyValuePair<ShockTile, string>(spawnedTile, "right")); }
                    break;
            }
        }
        _shockTileQueue = newQueue;
    }

    private ShockTile createShockTile(Vector2 targetPos)
    {
        if (!_stompableTile.ContainsKey(targetPos))
        {
            ShockTile spawnedTile = Instantiate(_shockTile, targetPos, Quaternion.identity, _gm.transform);
            spawnedTile.Init(_shockTileDelay, _shockTileDuration);
            _stompableTile[targetPos] = false;
            return spawnedTile;
        }
        return null;
    }

    public KeyValuePair<string, int> GenerateKeyPair(string str, int integer)
    {
        return new KeyValuePair<string, int>(str, integer);
    }

}


