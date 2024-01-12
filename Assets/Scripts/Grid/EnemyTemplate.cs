using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class EnemyGridMovement : MonoBehaviour
{
    private Vector3 playerPos, teleportPos;
    
    private float playerDistance;
    private readonly double chaseDistance = 7.5;
    private readonly double attackDistance = 3.5;
    private readonly double teleportDistance = 15.5;
    private int chaseCounter = 0;
    
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
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack1", 1));
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack1", 2));
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack1", 3));
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack1", 4));
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack1", 5));
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack1", 6));
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack1", 7));
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack1", 8));
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack1", 9));
            }
            else if (chaseCounter > 5 && playerDistance <= teleportDistance) //long ranged attack for when player keeps running
            {
                chaseCounter = 0;
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack2", 1));
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack2", 2));
                queuedMoves.Enqueue(_gm.GenerateKeyPair("attack2", 3));
            }
            else if (playerDistance >= teleportDistance)
            {
                print("unga bunga");
                teleportPos = playerPos;
                teleportPos.x += Random.Range(1, 5);
                teleportPos.y += Random.Range(1, 5);
                this.transform.position = teleportPos;
            }

            else //chase player
            {
                chaseCounter++;
                StartCoroutine(GameManager.Instance.Move(self, findBestMove(playerPos), 1f));
            }
        }
    }
}


