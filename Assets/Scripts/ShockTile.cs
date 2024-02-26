using UnityEngine;

public class ShockTile : MonoBehaviour
{


    private int _duration, _delay, _damage;

    private float _transparency;
    private bool _rendered = false;
    private string _direction;
    [SerializeField] private Sprite _shockTileSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Init(int delay, int duration, string direction, int damage, float transparency)
    {
        _delay = delay; //delay of 0 will render on the turn it is created
        _duration = duration; //duration <= 1 will destroy on the beginning of the next turn
        _direction = direction;
        _damage = damage;
        _transparency = transparency;
        
        if (_delay <= 0) { renderSprite(); } //set delay = 0 to render immediately
    }
    // Update is called once per frame
    void Update()
    {

    }

    void NextMove()
    {
        if (!_rendered) //Tile isn't rendered: check _delay, decrement if necessary, render sprite if delay <= 0
        {
            if (_delay > 1) { _delay -= 1; }
            else { renderSprite(); }
        }
        else //Tile is rendered: check _duration, decrement if necessary, destroy object if _duration <= 0
        {
            
            if (_duration > 1) { _duration -= 1; }
            else { 
                //print("Destroyed at " + DateTime.Now);
                Destroy(this.gameObject); }
        }

    }

    private void renderSprite()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, _transparency); //set transparency
        gameObject.GetComponent<SpriteRenderer>().sprite = _shockTileSprite;
        _rendered = true;
    }

    public bool getRendered(){
        return _rendered;
    }

    public string getDirection(){
        return _direction;
    }

    public int GetDamage(){
        if (_rendered){
            return _damage;
        } else {
            return 0;
        }
    }
}
