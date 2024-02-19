using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockTileGenerator : MonoBehaviour
{

    [SerializeField] private ShockTile _shockTilePrefab;
    private GridManager _gridManager;
    private GameManager _gm;
    private Queue<ShockTile> _shockTileQueue;
    private List<Vector2> _shockTileLocations, _unstompableTiles;
    private int _turnsActive, _delay,_shockTileDelay, _shockTileDuration, _shockTileDamage, _maxTurnsActive;
    private float _shockTileTransparency; //sprite transparency
    private const int _randomThreshold = 2; //number of turns until shockwave begins spreading randomly
    private bool _isRandom = false; 

    bool _isActive = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    public void Init(int delay, int maxTurns, int shockTileDelay, int shockTileDuration, float shockTileTransparency, int shockTileDamage)
    {

        _gridManager = GridManager.Instance;
        _gm = GameManager.Instance;
        _shockTileQueue = new Queue<ShockTile>();
        _unstompableTiles = new List<Vector2>();
        _shockTileLocations = new List<Vector2>();
        //generator arguments
        _delay = delay; //delay before initial shockwave appears
        //_shockTileQueue = shockTileQueue; //initial queue of shocktiles
        _maxTurnsActive = maxTurns; //number of turns before any remaining tiles are destroyed

        //child tile arguments
        _shockTileDelay = shockTileDelay;
        _shockTileDuration = shockTileDuration;
        _shockTileTransparency = shockTileTransparency;
        _shockTileDamage = shockTileDamage;

        if (_delay <= 0) {createShockWave(); _isActive = true;}
     }

    void NextMove()
    {

        if (!_isRandom && _turnsActive > _randomThreshold){
            _isRandom = true;
        }

        if (_isActive){
            if (_turnsActive < _maxTurnsActive && _shockTileQueue.Count > 0){ //Still turns left and queue isn't empty.
                _turnsActive += 1;
                shockWaveSearch(_isRandom);
            } else{Destroy(this.gameObject);} //No more shock tiles, destroy this object.
        } else{
            if (_delay > 0) {_delay -= 1;}
            else{createShockWave(); _isActive = true;}
        }

        
    }

    private void createShockWave()
    {
        //_stompableTile.Add(origin);
        Vector2 origin = this.transform.position; //generate spawn position
        Vector2 targetPos;
        ShockTile spawnedTile;
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++) //find the 3x3 tile grid around generator
            {
                _unstompableTiles.Add(origin + new Vector2(x, y));

                if (y == 1) //create shock tiles above boss
                {
                    //print("y == 1");
                    //print(origin + new Vector2(x, y + 1));
                    targetPos = origin + new Vector2(x, y + 1);
                    spawnedTile = createShockTile(targetPos, "up");
                    if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); }

                    if (x == 1) //create top right corner tiles
                    {
                        targetPos = origin + new Vector2(x + 1, y + 1);
                        spawnedTile = createShockTile(targetPos, "up");
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); }
                        spawnedTile = createShockTile(targetPos, "right", true);//force instantiate second corner tile
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); }
                    }
                    else if (x == -1) //top left corner tiles
                    {
                        targetPos = origin + new Vector2(x - 1, y + 1);
                        spawnedTile = createShockTile(targetPos, "up");
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); }
                        spawnedTile = createShockTile(targetPos, "left", true); //force instantiate second corner tile
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); }
                    }
                }
                else if (y == -1) //create shock tiles under boss
                {
                    //print("y == -1");
                    targetPos = origin + new Vector2(x, y - 1);
                    spawnedTile = createShockTile(targetPos, "down");
                    if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); }

                    if (x == 1) //create bottom right corner tiles
                    {
                        targetPos = origin + new Vector2(x + 1, y - 1);
                        spawnedTile = createShockTile(targetPos, "down");
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); }
                        spawnedTile = createShockTile(targetPos, "right", true); //force instantiate second corner tile
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); }

                    }
                    else if (x == -1) //create bottom left corner tiles
                    {
                        targetPos = origin + new Vector2(x - 1, y - 1);
                        spawnedTile = createShockTile(targetPos, "down");
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); }
                        spawnedTile = createShockTile(targetPos, "left", true); //force instantiate second corner tile
                        if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); }
                    }
                }


                if (x == -1)//create shock tiles left of boss
                {
                    //print("x == -1");
                    targetPos = origin + new Vector2(x - 1, y);
                    spawnedTile = createShockTile(targetPos, "left");
                    if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); }
                }
                else if (x == 1)//create shock tiles right of boss
                {
                    //print("x == 1");
                    targetPos = origin + new Vector2(x + 1, y);
                    spawnedTile = createShockTile(targetPos, "right");
                    if (spawnedTile) { _shockTileQueue.Enqueue(spawnedTile); }
                }
            }
        }

    }

    private void shockWaveSearch(bool random = false, int maxDistance = 2)
    {
        Queue<ShockTile> newQueue = new Queue<ShockTile>();
        ShockTile curTile;
        Vector2 curTilePos;
        Vector2 randomModifier = Vector2.zero;
        Vector2 scatter;

        while (_shockTileQueue.Count > 0)
        {
            curTile = _shockTileQueue.Dequeue();
            curTilePos = curTile.transform.position;

            for (int i = -1; i < 2; i++)
            {
                scatter = Vector2.zero; //used to check adjacent tiles
                ShockTile spawnedTile = null;
                if (random) { randomModifier = generateRandomModifier(maxDistance, curTile.getDirection()); }

                switch (curTile.getDirection())
                {
                    case "up":
                        scatter.x += i;
                        spawnedTile = createShockTile(curTilePos + Vector2.up + randomModifier + scatter, "up");
                        break;
                    case "down":
                        scatter.x += i;
                        spawnedTile = createShockTile(curTilePos + Vector2.down + randomModifier + scatter, "down");
                        break;
                    case "left":
                        scatter.y += i;
                        spawnedTile = createShockTile(curTilePos + Vector2.left + randomModifier + scatter, "left");
                        break;
                    case "right":
                        scatter.y += i;
                        spawnedTile = createShockTile(curTilePos + Vector2.right + randomModifier + scatter, "right");
                        break;
                    default:
                        break;
                }
                if (spawnedTile) { newQueue.Enqueue(spawnedTile); }
            }
            _shockTileLocations.Remove(curTilePos);
        }
        if (newQueue.Count <=  0) //end of shockwave attack
        {
            _unstompableTiles.Clear();
            _turnsActive = 0;
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
                spawnedTile.Init(_shockTileDelay, _shockTileDuration, direction, _shockTileDamage, _shockTileTransparency);
            }
        }
        else
        { //Force instantiate ignores some instantiation requirements (mostly for corner pieces)
            if (_gridManager.isInsideGrid(targetPos) && _gridManager.GetTileAtPosition(targetPos) != "Wall")
            {
                spawnedTile = Instantiate(_shockTilePrefab, targetPos, Quaternion.identity, _gm.transform);
                spawnedTile.Init(_shockTileDelay, _shockTileDuration, direction, _shockTileDamage, _shockTileTransparency);
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
            randomModifier.x = UnityEngine.Random.Range(0, 1);
            randomModifier.y = UnityEngine.Random.Range(maxDistance * -1, maxDistance);
        }
        else
        {
            randomModifier.y = UnityEngine.Random.Range(0, 1);
            randomModifier.x = UnityEngine.Random.Range(maxDistance * -1, maxDistance);

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

}
