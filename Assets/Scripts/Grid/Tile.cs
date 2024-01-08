using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

  
    private GameManager _gm;

    private void Awake()
    {
        _gm = GameManager.Instance;
        if (_gm == null)
        {
            Debug.Log("Tile could not find GameManager");
        }
    }

    
    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
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