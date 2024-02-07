using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DangerSmall : MonoBehaviour
{

    private int _delay, _duration;
    private bool rendered = false;
    private Sprite _sprite;
    [SerializeField] private Sprite _dangerSmall;
    [SerializeField] private Sprite _dangerLarge;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(int delay, int duration, int spriteChoice)
    {   

        _delay = delay;
        _duration = duration;

        switch(spriteChoice){

            case 0:
                _sprite = _dangerSmall;
                break;
            case 1:
                _sprite = _dangerLarge;
                break;
            default:
                print("Invalid sprite, defaulting to dangersmall");
                _sprite = _dangerSmall;
                break;
        }
        if (_delay <= 0 && _duration > 0) { renderSprite(); }

    }

    // Update is called once per frame
    void Update()
    {

    }

    void NextMove()
    {
        if (!rendered) //Warning isn't rendered: check _delay, decrement if necessary, render sprite if delay <= 0
        {
            if (_delay > 0) { _delay -= 1; }
            else { renderSprite(); }
        }
        else //Warning is rendered: check _duration, decrement if necessary, destroy object if _duration <= 0
        {
            
            if (_duration > 0) { _duration -= 1; }
            else { 
                //print("Destroyed at " + DateTime.Now);
                Destroy(this.gameObject); }
        }
    }

    private void renderSprite()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = _sprite;
        rendered = true;
    }
}
