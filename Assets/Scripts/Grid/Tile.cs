using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private bool _isOffset;

  
    private GameManager _gm;

    private void Awake()
    {
        _gm = GameManager.Instance;
        if (_gm == null)
        {
            Debug.Log("Tile could not find GameManager");
        }
    }

    private void Update()
    {

        if (!_gm._pauseGame)
        {
            if (Input.GetKeyDown("space")) 
            {
                _renderer.color = Color.clear;
            }
            else if (Input.GetKeyUp("space"))
            {
                _renderer.color = _isOffset ? _offsetColor : _baseColor;
            }
        }
        else 
        {
            _highlight.SetActive(false);
            _renderer.color = _isOffset ? _offsetColor : _baseColor;
        }

    }

    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
        _isOffset = isOffset;

    }

    void OnMouseEnter()
    {
        if(_gm._pauseGame == false)
        {
            _highlight.SetActive(true);
        }  
    }

    void OnMouseExit()
    {
        if (_gm._pauseGame == false)
        {
            _highlight.SetActive(false);
        }
    }

}