using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DangerSmall : MonoBehaviour
{

    private int _delay, _duration;

    private int _initialDuration;
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
        _initialDuration = duration;

        switch (spriteChoice)
        {

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
        if (_delay <= 0) { renderSprite(); }

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
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void renderSprite()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        gameObject.GetComponent<SpriteRenderer>().sprite = _sprite;

        rendered = true;
    }

    private void setTransparency(float value){
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, value);
    }
}
