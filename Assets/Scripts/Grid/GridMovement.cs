using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    private bool isMoving;
    private Vector3 origPos, targetPos;
    private float timeToMove = 0.05f;
    private float timeToWait = 0.150f;
    private GameManager _gm;

    [SerializeField] private Transform _cam;

    private void Awake()
    {
        _gm = GameManager.Instance;
    }
    void Update()
    {
        if (!isMoving)
        {
            if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.UpArrow))) StartCoroutine(MovePlayer(Vector3.up));
            if ((Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.LeftArrow))) StartCoroutine(MovePlayer(Vector3.left));
            if ((Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.DownArrow))) StartCoroutine(MovePlayer(Vector3.down));
            if ((Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.RightArrow))) StartCoroutine(MovePlayer(Vector3.right));
        }

        //_cam.transform.position = transform.position+(new Vector3(0,0,-15));
    }

    private IEnumerator MovePlayer(Vector3 direction)
    {

        _gm.SendSignalMove();
        isMoving = true;

        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction;

        while(elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; //this line prevents tiny offsets from adding up after many movements, keeping player on grid

        while (elapsedTime < (timeToMove + timeToWait))
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
    }

    public bool moveStatus()
    {
        return isMoving;
    }
}

