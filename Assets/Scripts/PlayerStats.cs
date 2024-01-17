using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int _playerHP = 100;
    public int _playerMP = 100;
    public int _playerSoul = 100;
    public int _playerDmg = 50;
    public const int rollCoolDown = 5;
    private GameManager _gm;
    private int rollTimer;

    private float playerMoveDistance = 1f;

    const float timeToMove = 0.15f;
    const float timeToWait = 0.05f;

    private bool isMoving;

    private bool isFacingRight = true;
    private bool flippedRight = false;
    private bool flippedLeft = false;

    [SerializeField] private Sprite _petah, _cloak, _mask1, _mask2, _mask3;

    void Start()
    {
        _gm = GameManager.Instance;
        setSprite(_petah);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            if (Input.GetKey(KeyCode.Space) && rollTimer <= 0) //check if rolling
            {
                playerMoveDistance = 2f;
            }
            else
            {
                playerMoveDistance = 1f;
            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                StartCoroutine(Move(Vector3.up, playerMoveDistance));
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                StartCoroutine(Move(Vector3.left, playerMoveDistance));
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                StartCoroutine(Move(Vector3.down, playerMoveDistance));
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                StartCoroutine(Move(Vector3.right, playerMoveDistance));
            }
        }
    }

    public IEnumerator Move(Vector3 direction, float distance)
    {
        Vector3 origPos, targetPos;
        bool hitWall = false;
        float elapsedTime = 0;

        int raycastLength = 1;

        if (rollTimer > 0)
        {
            distance = 1f;
            rollTimer -= 1;
        }

        int divisor = (distance == 2f) ? divisor = 1 : divisor = 2;

        origPos = gameObject.transform.position;
        targetPos = origPos + (direction * distance);

        //Debug.DrawRay(origPos + direction, direction, Color.green, 2);
        RaycastHit2D hit;

        if (hit = Physics2D.Raycast(origPos + direction, direction, raycastLength / divisor))
        {
            print(hit.collider.gameObject.tag);
            if (hit.collider.gameObject.tag == "Wall")
            {
                if (Vector3.Distance(transform.position, hit.collider.gameObject.transform.position) > 1)
                { //check if distance > 1 unit, try next closest tile.
                    StartCoroutine(Move(direction, distance - 1));
                    yield break;
                }
                hitWall = true;
            }
        }

        if (distance > 1f && rollTimer <= 0)
        {
            rollTimer = rollCoolDown;
        }
        isMoving = true;
        _gm.SendSignalMove();

        while (elapsedTime < timeToMove)
        {
            if (!hitWall) gameObject.transform.position = Vector3.Lerp(origPos, targetPos, elapsedTime / timeToMove);
            if (distance == 2f) //spiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiin
            {
                float rollDegrees;
                if (direction == Vector3.left && isFacingRight == true) rollDegrees = 360f; 
                else if (direction == Vector3.right && isFacingRight == false) rollDegrees = 360f; 
                else rollDegrees = -360f; 

                float yAngle = gameObject.transform.eulerAngles.y;
                float rotZ = Mathf.Lerp(0, rollDegrees, elapsedTime / timeToMove + timeToWait);
                gameObject.transform.rotation = Quaternion.Euler(0, yAngle, rotZ);
            }
            else
            {
                float origRot;
                if (isFacingRight) origRot = 0f;
                else origRot = 180f;

                float flipToDegree;
                if (!((direction == Vector3.right && isFacingRight == true) || (direction == Vector3.left && isFacingRight == false)))
                {
                    if (direction == Vector3.right && isFacingRight == false) { flipToDegree = 0; flippedRight = true; }
                    else if (direction == Vector3.left && isFacingRight == true) { flipToDegree = 180; flippedLeft = true; }
                    else flipToDegree = float.NegativeInfinity;

                    if(flipToDegree != float.NegativeInfinity) {
                        float rotY = Mathf.Lerp(origRot, flipToDegree, elapsedTime / timeToMove);
                        gameObject.transform.rotation = Quaternion.Euler(0, rotY, 0);
                        print(rotY);
                    }
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!hitWall) gameObject.transform.position = targetPos; //this line prevents tiny offsets from adding up after many movements, keeping player on grid

        if (flippedRight)
        {
            float zAngle = gameObject.transform.eulerAngles.z;
            gameObject.transform.rotation = Quaternion.Euler(0, 0, zAngle);
            isFacingRight = true;
            flippedRight = false;
        }
        if (flippedLeft)
        {
            float zAngle = gameObject.transform.eulerAngles.z;
            gameObject.transform.rotation = Quaternion.Euler(0, 180, zAngle);
            isFacingRight = false;
            flippedLeft = false;
        }

        while (elapsedTime < (timeToMove + timeToWait))
        {

            //this block doesn't have to be here but it makes spiiiiiiiiiiin look a bit better in exchange for terrible optimization (gives extra time to spiiin)
            //DON'T FORGET if we decide to remove it to delete "+ timeToWait" in twin block above
            if (distance == 2f)
            {
                float rollDegrees;
                if (direction == Vector3.left && isFacingRight == true) rollDegrees = 360f;
                else if (direction == Vector3.right && isFacingRight == false) rollDegrees = 360f;
                else rollDegrees = -360f;

                float yAngle = gameObject.transform.eulerAngles.y;
                float rotZ = Mathf.Lerp(0, rollDegrees, elapsedTime / timeToMove + timeToWait);
                gameObject.transform.rotation = Quaternion.Euler(0, yAngle, rotZ);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (distance == 2f)
        {
            float yAngle = gameObject.transform.eulerAngles.y;
            gameObject.transform.rotation = Quaternion.Euler(0, yAngle, 0);
        }
        isMoving = false;
    }

    private void setSprite(Sprite sprite)
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
