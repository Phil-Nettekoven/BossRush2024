using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    [SerializeField] private Sprite _baseSprite, _offsetSprite;
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
        if (_gm._pauseGame == false)
        {
            toggleGrid();
        }
        else
        {
            _highlight.SetActive(false);

            //_renderer.color = _isOffset ? _offsetColor : _baseColor;
            this.gameObject.GetComponent<SpriteRenderer>().sprite = _isOffset ? _offsetSprite : _baseSprite;
        }

    }

    public void Init(bool isOffset)
    {
        //_renderer.color = isOffset ? _offsetColor : _baseColor;
        this.gameObject.GetComponent<SpriteRenderer>().sprite = _isOffset ? _offsetSprite : _baseSprite;
        _isOffset = isOffset;

    }

    void toggleGrid()
    {


            if (Input.GetKey(KeyCode.LeftControl))
            {
                //_renderer.color = Color.clear;
            }
            else
            {
            //_renderer.color = _isOffset ? _offsetColor : _baseColor;
            this.gameObject.GetComponent<SpriteRenderer>().sprite = _isOffset ? _offsetSprite : _baseSprite;
        }

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