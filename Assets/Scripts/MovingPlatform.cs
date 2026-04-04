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
        startPosition = transform.localPosition;
        targetPosition = startPosition + moveOffset;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
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