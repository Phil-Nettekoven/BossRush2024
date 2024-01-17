using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Wall _wallPrefab;

    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, Tile> _tiles;
    private Dictionary<Vector2, Wall> _walls;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        _walls = new Dictionary<Vector2, Wall>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (x == 0 || x == _width-1 || y == 0 || y == _height-1)
                {
                    var spawnedWall = Instantiate(_wallPrefab, new Vector3(x, y, -0.1f), Quaternion.identity);
                    spawnedWall.name = $"Wall {x} {y}";
                    _walls[new Vector2(x, y)] = spawnedWall;
                }
                else
                {
                    var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                    spawnedTile.name = $"Tile {x} {y}";

                    var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                    spawnedTile.Init(isOffset);

                    _tiles[new Vector2(x, y)] = spawnedTile;
                }
            }
        }

        //_cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -17);
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
}