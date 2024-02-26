using System.Collections;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int _playerMaxHP, _playerMaxMP, _playerMaxSoul, _playerDmg;

    public float _playerHP;
    public float _playerMP;
    public float _playerSoul;

    public const int rollCoolDown = 5;
    private GameManager _gm;
    private GridManager _gridManager;
    private int rollTimer;

    private float playerMoveDistance = 1f;

    const float _timeToMove = 0.15f;
    const float _timeToWait = 0.05f;

    private bool _isMoving = false;
    private bool isRolling = false;

    private bool isFacingRight = true;
    private bool flippingRight = false;
    private bool flippingLeft = false;

    private int _stunCounter = -1;

    [SerializeField] private Sprite _petah, _cloak, _mask1, _mask2, _mask3;

    void Start()
    {
        _gm = GameManager.Instance;
        _gridManager = GridManager.Instance;
        setSprite(_petah);
        _playerHP = _playerMaxHP;
        _playerSoul = _playerMaxSoul;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isMoving)
        {
            //If not already moving
            if (Input.GetKey(KeyCode.Space) && rollTimer <= 0)
            {
                //If rolling
                isRolling = true;
                playerMoveDistance = 2f;
            }
            else
            {
                isRolling = false;
                playerMoveDistance = 1f;
            }

            //Tell player to move
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) StartCoroutine(MoveWrapper(Vector3.up, playerMoveDistance));
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) StartCoroutine(MoveWrapper(Vector3.left, playerMoveDistance));
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) StartCoroutine(MoveWrapper(Vector3.down, playerMoveDistance));
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) StartCoroutine(MoveWrapper(Vector3.right, playerMoveDistance));
        }
    }
    private IEnumerator MoveWrapper(Vector3 direction, float distance)
    {
        _isMoving = true;
        if (_stunCounter > 0)
        {
            _gm.SendSignalMove();
            yield return new WaitForSeconds(_timeToWait + _timeToMove);
            _stunCounter -= 1;
        }
        else
        {
            yield return StartCoroutine(Move(direction, distance));
        }
        _isMoving = false;
    }
    private IEnumerator Move(Vector3 direction, float distance)
    {

        if (isRolling && rollTimer <= 0) rollTimer = rollCoolDown;
        float elapsedTime = 0;

        float origRot = 180f;
        if (isFacingRight) origRot = 0f;

        //I don't like this block but its working so I'm afraid to touch it
        float targetRot = 0;
        bool needsToFlip = false;
        if (!isRolling && !((direction == Vector3.right && isFacingRight == true) || (direction == Vector3.left && isFacingRight == false)))
        {
            //If not facing same direction as movement and also not rolling
            if (direction == Vector3.right && isFacingRight == false)
            {
                //If player needs to flip right
                needsToFlip = true;
                flippingRight = true;
                //targetRot is already 0, so no need to alter
            }
            else if (direction == Vector3.left && isFacingRight == true)
            {
                //If player needs to flip left
                needsToFlip = true;
                flippingLeft = true;
                targetRot = 180; //Alter target rotation to reflect flip direction
            }
        }

        Vector3 origPos = gameObject.transform.position; //Acquire starting position
        Vector3 targetPos = origPos + (direction * distance); //Calculate desired position


        while (distance > 0 && (!_gridManager.isInsideGrid(targetPos) || _gridManager.GetTileAtPosition(targetPos) == "Wall"))
        {
            distance -= 1;
            targetPos = origPos + (direction * distance);
        }

        while (elapsedTime < _timeToMove)
        {
            //While the player has time to move
            //Move towards target position (if able to move)
            gameObject.transform.position = Vector3.Lerp(origPos, targetPos, elapsedTime / _timeToMove);

            if (isRolling)
            {
                //If rolling
                //Determine direction of roll and angle to roll until
                //Negative values roll forward, positive values roll backwords
                float rollDegrees = -360f; //Default roll forwards 360 degrees
                if (direction == Vector3.left && isFacingRight == true) rollDegrees = 360f; //If rolling left while facing right -> backflip
                else if (direction == Vector3.right && isFacingRight == false) rollDegrees = 360f; //If rolling right and facing left -> backflip

                float yRot = gameObject.transform.eulerAngles.y; //Used to maintain y rotation
                float zRot = Mathf.Lerp(0, rollDegrees, elapsedTime / _timeToMove); //Determine amount to rotate this frame
                gameObject.transform.rotation = Quaternion.Euler(0, yRot, zRot); //Rotate
            }
            else if (needsToFlip)
            {
                //If player didn't roll and needs to flip
                float yRot = Mathf.Lerp(origRot, targetRot, elapsedTime / _timeToMove); //Determine amount to flip this frame
                gameObject.transform.rotation = Quaternion.Euler(0, yRot, 0); //Flip
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //This block prevents tiny offsets from adding up after many movements/rotations, keeping player on grid

        //If player was able to move
        gameObject.transform.position = targetPos; //Set position to desired position

        if (needsToFlip)
        {
            //If player needs to flip
            if (flippingRight)
            {
                //If player is flipping to the right
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0); //Set (y) rotation to desired rotation. NOTE: targetRot is intentionally not used here
                isFacingRight = true;
                flippingRight = false; //Reset direction tracker
            }

            if (flippingLeft)
            {
                //If player is flipping to the left
                gameObject.transform.rotation = Quaternion.Euler(0, 180, 0); //Set (y) rotation to desired rotation. NOTE: targetRot is intentionally not used here
                isFacingRight = false;
                flippingLeft = false; //Reset direction tracker
            }
            needsToFlip = false; //Flip is complete
        }

        if (isRolling)
        {
            //If player rolled
            float yRot = gameObject.transform.eulerAngles.y; //Used to maintain y rotation
            gameObject.transform.rotation = Quaternion.Euler(0, yRot, 0); //Set (z) rotation to desired rotation
            isRolling = false; //Roll is complete
        }


        while (elapsedTime < (_timeToMove + _timeToWait))
        {
            //While the player waits to move again
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (rollTimer > 0) rollTimer -= 1;
        _gm.SendSignalMove();
    }

    /// <summary>
    /// Sets the player's sprite to given sprite
    /// </summary>
    /// <param name="sprite">Selected sprite you'd like the player to have</param>
    private void setSprite(Sprite sprite)
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public int getDamage()
    {
        return _playerDmg;
    }

    public void DamagePlayer(float damage)
    { //subtract damage from health
        _playerHP -= damage;
        print(_playerHP);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.transform.tag)
        {
            case "EnemyDmg":
                DamagePlayer(collider.GetComponent<ShockTile>().GetDamage());
                break;
            default:
                break;
        }

    }
}
