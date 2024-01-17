using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Search;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    private Vector3 playerPos, teleportPos;

    private float playerDistance;
    private readonly double chaseDistance = 7.5;
    private readonly double attackDistance = 3.5;
    private readonly double teleportDistance = 15.5;
    private int chaseCounter = 0;
    const float timeToMove = 0.15f;
    const float timeToWait = 0.05f;

    private bool playerFound = false;

    private GameManager _gm;
    private Queue<KeyValuePair<string, int>> queuedMoves;

    public GameObject Player;

    public GameObject self;

    private void Awake()
    {
        _gm = GameManager.Instance;
        Player = GameObject.Find("Player");
        queuedMoves = new Queue<KeyValuePair<string, int>>();
    }

    void Update()
    {

    }

    private Vector3 findBestMove(Vector3 PlayerPosition)
    {
        //diff = Player.transform.position - this.transform.position;
        Vector3 bestMove = this.transform.position;
        Vector3 currentMove;
        Vector3 direction = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                currentMove = this.transform.position + Vector3.up;
                if (Vector3.Distance(currentMove, PlayerPosition) < Vector3.Distance(bestMove, PlayerPosition))
                {
                    bestMove = currentMove;
                    direction = Vector3.up;
                }
            }
            else if (i == 1)
            {
                currentMove = this.transform.position + Vector3.down;
                if (Vector3.Distance(currentMove, PlayerPosition) < Vector3.Distance(bestMove, PlayerPosition))
                {
                    bestMove = currentMove;
                    direction = Vector3.down;
                }
            }
            else if (i == 2)
            {
                currentMove = this.transform.position + Vector3.left;
                if (Vector3.Distance(currentMove, PlayerPosition) < Vector3.Distance(bestMove, PlayerPosition))
                {
                    bestMove = currentMove;
                    direction = Vector3.left;
                }
            }
            else if (i == 3)
            {
                currentMove = this.transform.position + Vector3.right;
                if (Vector3.Distance(currentMove, PlayerPosition) < Vector3.Distance(bestMove, PlayerPosition))
                {
                    bestMove = currentMove;
                    direction = Vector3.right;
                }
            }


        }

        return direction;
    }

    private void NextMove()
    {
        //print(queuedMoves.Count);
        playerPos = Player.transform.position;
        playerDistance = Vector3.Distance(this.transform.position, playerPos);
        if (queuedMoves.Count > 0)
        {
            print(queuedMoves.Dequeue().ToString());
        }
        else if ((playerDistance <= chaseDistance) || playerFound) //Enemy is idle and player is within chaseDistance OR player has been previously found
        {
            playerFound = true;
            if (playerDistance <= attackDistance) //add attack to movement queue
            {
                for (int i = 0; i < 9; i++)
                {
                    queuedMoves.Enqueue(GenerateKeyPair("attack1", i));
                }
            }
            else if (chaseCounter > 5 && playerDistance <= teleportDistance) //long ranged attack for when player keeps running
            {
                chaseCounter = 0;
                for (int i = 0; i < 3; i++)
                {
                    queuedMoves.Enqueue(GenerateKeyPair("attack2", i));
                }
            }
            else if (playerDistance >= teleportDistance)
            {
                //print("unga bunga");
                teleportPos = playerPos;
                teleportPos.x += Random.Range(1, 5);
                teleportPos.y += Random.Range(1, 5);
                this.transform.position = teleportPos;
            }

            else //chase player
            {
                chaseCounter++;
                StartCoroutine(Move(findBestMove(playerPos), 1f));
            }
        }
    }


    public KeyValuePair<string, int> GenerateKeyPair(string str, int integer)
    {
        return new KeyValuePair<string, int>(str, integer);
    }

        public IEnumerator Move(Vector3 direction, float distance)
    {
        Vector3 origPos, targetPos;
        bool hitWall = false;
        float elapsedTime = 0;

        int raycastLength = 1;

        Quaternion origRot, targetRot;

        int divisor = (distance == 2f) ? divisor = 1 : divisor = 2;

        origPos = gameObject.transform.position;
        targetPos = origPos + (direction * distance);

        origRot = gameObject.transform.rotation;
        targetRot = origRot * Quaternion.Euler(0, 0, 360);

        //Debug.DrawRay(origPos + direction, direction, Color.green, 2);
        RaycastHit2D hit;

        if (hit = Physics2D.Raycast(origPos + direction, direction, raycastLength / divisor))
        {
            print(hit.collider.gameObject.tag);
            if (hit.collider.gameObject.tag == "Wall")
            {
                if (Vector3.Distance(gameObject.transform.position, hit.collider.gameObject.transform.position) > 1)
                { //check if distance > 1 unit, try next closest tile.
                    StartCoroutine(Move(direction, distance - 1));
                    yield break;
                }
                hitWall = true;
            }
        }

        while (elapsedTime < timeToMove)
        {
            if (!hitWall) gameObject.transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            if (distance == 2f) //spiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiin
            {
                float rollDegrees;
                if (direction == Vector3.left || direction == Vector3.down) rollDegrees = 360f;
                else rollDegrees = -360f;
                float rot = Mathf.Lerp(0, rollDegrees, (elapsedTime / timeToMove + timeToWait));
                gameObject.transform.rotation = Quaternion.Euler(0, 0, rot);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!hitWall) gameObject.transform.position = targetPos; //this line prevents tiny offsets from adding up after many movements, keeping player on grid
        if (distance == 2) gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        while (elapsedTime < (timeToMove + timeToWait))
        {

            //this block doesn't have to be here but it makes spiiiiiiiiiiin look a bit better in exchange for terrible optimization (gives extra time to spiiin)
            //DON'T FORGET if we decide to remove it to delete "+ timeToWait" in twin block above
            if (distance == 2f)
            {
                float rollDegrees;
                if (direction == Vector3.left || direction == Vector3.down) rollDegrees = 360f;
                else rollDegrees = -360f;
                float rot = Mathf.Lerp(0, rollDegrees, (elapsedTime / timeToMove + timeToWait));
                gameObject.transform.rotation = Quaternion.Euler(0, 0, rot);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }
}


