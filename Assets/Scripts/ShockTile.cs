using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockTile : MonoBehaviour
{


    private int _duration, _delay;
    private bool rendered = false;
    [SerializeField] private Sprite _shockTileSprite;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void Init(int delay, int duration)
    {
        _delay = delay;
        _duration = duration;
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
        print("RENDERED");
        this.gameObject.GetComponent<SpriteRenderer>().sprite = _shockTileSprite;
        rendered = true;
    }
}
