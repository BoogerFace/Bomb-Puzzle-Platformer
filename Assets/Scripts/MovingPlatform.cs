using UnityEngine;

public class MovingPlatform : TriggerableObject
{
    [Header("Movement Settings")]
    public Vector3 moveOffset = new Vector3(0, 5, 0);
    public float moveSpeed = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingForward = true;
    private bool isMoving = true; // Starts moving

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + moveOffset;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            if (movingForward)
            {
                targetPosition = startPosition;
            }
            else
            {
                targetPosition = startPosition + moveOffset;
            }

            movingForward = !movingForward;
        }
    }

    public override void Triggered()
    {
        isMoving = !isMoving; // Toggle
        Debug.Log("Moving Platform toggled");
    }
}