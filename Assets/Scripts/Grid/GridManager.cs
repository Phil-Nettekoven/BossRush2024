using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Wall _wallPrefab;

    [SerializeField] private Transform _cam;

    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }

    private Dictionary<Vector2, TileHolder> _tiles;
    //private Dictionary<Vector2, Wall> _walls;

    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        GenerateGrid();
    }

    public struct TileHolder
    {
        public TileHolder(Wall newWall)
        {
            wall = newWall;
            tile = null;
        }

        public TileHolder(Tile newTile)
        {
            wall = null;
            tile = newTile;
        }

        public string getContents()
        {
            if (!wall) { return tile.tag; }
            else { return wall.tag; }
        }

        Wall wall;
        Tile tile;

    }
    void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, TileHolder>();
        //_walls = new Dictionary<Vector2, TileHolder>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (x == 0 || x == _width - 1 || y == 0 || y == _height - 1)
                {
                    Wall spawnedWall = Instantiate(_wallPrefab, new Vector3(x, y), Quaternion.identity);
                    spawnedWall.name = $"Wall {x} {y}";
                    _tiles[new Vector2(x, y)] = new TileHolder(spawnedWall);
                }
                else
                {
                    Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                    spawnedTile.name = $"Tile {x} {y}";

                    var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                    spawnedTile.Init(isOffset);

                    _tiles[new Vector2(x, y)] = new TileHolder(spawnedTile);
                }
            }
        }

        //_cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -17);
    }

    public string GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out TileHolder tile)) return tile.getContents();
        return null;
    }
}