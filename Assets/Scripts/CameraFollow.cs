using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    private float smoothSpeed;
    private float minimumSpeed = 1f;
    private Vector3 offset = new Vector3(0,0,-15);
    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;
    private float distanceToTarget;

    void LateUpdate()
    {
        desiredPosition = target.position + offset;
        distanceToTarget = Vector3.Distance(desiredPosition, transform.position);

        if(distanceToTarget > 0.01f)
        {
            if(distanceToTarget > minimumSpeed) smoothSpeed = distanceToTarget;
            else smoothSpeed = minimumSpeed;
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        else if (distanceToTarget < 0.01f && distanceToTarget > 0) transform.position = desiredPosition;
    }
}
