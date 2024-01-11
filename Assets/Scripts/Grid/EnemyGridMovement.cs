using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGridMovement : MonoBehaviour
{
    private bool isMoving;
    private Vector3 origPos, targetPos, playerPos;
    private float timeToMove = 0.05f;
    private float timeToWait = 0.00f;
    private float playerDistance;
    private GameManager _gm;

    private Queue<KeyValuePair<string, int>> queuedMoves;
    [SerializeField] private Transform _cam;

    public GameObject Player;
    private void Awake()
    {
        _gm = GameManager.Instance;
        Player = GameObject.Find("Player");
        queuedMoves = new Queue<KeyValuePair<string, int>>();
    }

    void Update()
    {

        //_cam.transform.position = transform.position+(new Vector3(0,0,-15));
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

    private IEnumerator MoveEnemy(Vector3 direction)
    {
        //isMoving = true;
        //yield return new WaitForSeconds(3);
        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction;

        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; //this line prevents tiny offsets from adding up after many movements, keeping enemy on grid

        while (elapsedTime < (timeToMove + timeToWait))
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //isMoving = false;
    }

    private void NextMove()
    {
        print(queuedMoves.Count);
        playerPos = Player.transform.position;
        playerDistance = Vector3.Distance(this.transform.position, playerPos);
        
        if (queuedMoves.Count <= 0 && playerDistance < 5)
        {
            if (playerDistance <= 2.5) //begin attack
            {
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack1", 1));
            }
            
            else if (playerDistance <= 7.5) //chase player
            {
                StartCoroutine(MoveEnemy(findBestMove(playerPos)));
            }
        }
    }
}


