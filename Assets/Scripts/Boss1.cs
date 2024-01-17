using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Search;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    private Vector3 playerPos;

    private float playerDistance;
    private int postStompDelay = 3;
    const float timeToMove = 0.15f;
    const float timeToWait = 0.05f;

    private bool playerFound = false;

    private GameManager _gm;
    private Queue<KeyValuePair<string, int>> queuedMoves;

    public GameObject Player;

    public GameObject MainCamera;

    public GameObject self;

    private Vector3 stompTarget;

    private void Awake()
    {
        _gm = GameManager.Instance;
        Player = GameObject.Find("Player");
        MainCamera = GameObject.Find("Main Camera");
        queuedMoves = new Queue<KeyValuePair<string, int>>();
        for (int i = 0; i < 10; i++)
        { //boss does nothing for 10 turns
            queuedMoves.Enqueue(GenerateKeyPair("idle", i));
        }
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
        playerDistance = Vector3.Distance(transform.position, playerPos);

        if (queuedMoves.Count > 0 && queuedMoves.Peek().Key == "idle" && playerDistance < 8)
        { //Player has entered fight radius
            queuedMoves.Clear();
        }
        else if (queuedMoves.Count > 0) //dequeue current moves
        {
            interpretMove(queuedMoves.Dequeue());
        }
        else if (postStompDelay <= 0 && playerDistance <= 10) //Add "stomp" attack to queue
        {
            for (int i = 0; i < 15; i++)
            {
                queuedMoves.Enqueue(GenerateKeyPair("stomp", i));
            }
        }
        else if (postStompDelay <= 0 && playerDistance > 10)
        {
            for (int i = 0; i < 5; i++)
            {
                queuedMoves.Enqueue(GenerateKeyPair("ranged1", i));
                postStompDelay = 0;
            }
        }
        else
        {
            postStompDelay -= 1;
        }
    }

    private void interpretMove(KeyValuePair<string, int> move)
    {
        switch (move.Key)
        {
            case "stomp":
                print("STOMP");
                Stomp(move.Value);
                break;
            case "ranged1":
                print("RANGED");
                break;
            default:
                print("frick");
                break;
        }
    }

    private void Stomp(int step)
    {
        switch (step)
        {
            case 0: //Fly into air
                Vector3 temp = Player.transform.position;
                temp.z = -20;
                StartCoroutine(JumpUp(temp));
                break;
            case 3: //Telegraph attack (exclamation marker)
                stompTarget = playerPos;
                break;
            case 7: //Crash down (damage on impact zone)
                StartCoroutine(JumpDown(stompTarget));
                break;
            case 10: //shockwave 1 (lesser damage to immediate surroundings)
                break;
            case 12: //shockwave 2 (lesser lesser damage to further surroundings)
                break;
            case 14: //end
                postStompDelay = 15;
                break;
            default: //do nothing
                break;
        }
    }

    private IEnumerator JumpDown(Vector3 targetPos) {
        float elapsedTime = 0;
        Vector3 origPos = transform.position;
        while (elapsedTime < timeToMove){
            transform.position = Vector3.Lerp(origPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        //yield break;
    }

    private IEnumerator JumpUp(Vector3 targetPos) {
        float elapsedTime = 0;
        Vector3 origPos = transform.position;
        while (elapsedTime < timeToMove){
            transform.position = Vector3.Lerp(origPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //transform.position = stompTarget;
        //yield break;
    }

    public KeyValuePair<string, int> GenerateKeyPair(string str, int integer)
    {
        return new KeyValuePair<string, int>(str, integer);
    }

}


