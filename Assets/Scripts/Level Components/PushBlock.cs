using UnityEngine;
using System.Collections;

public class PushBlock : MonoBehaviour
{
    [Header("Push Settings")]
    public float moveDistance = 2f;      // Distance block moves per push
    public float moveSpeed = 4f;         // Speed of movement
    public LayerMask obstacleLayer;      // Walls or objects that block movement

    private bool isMoving = false;

    private void OnCollisionStay(Collision collision)
    {
        if (isMoving)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        // Direction from player to block
        Vector3 pushDirection = transform.localPosition - collision.transform.localPosition;

        pushDirection.y = 0;
        pushDirection.Normalize();

        Vector3 moveDir = GetCardinalDirection(pushDirection);

        TryMove(moveDir);
    }

    void TryMove(Vector3 direction)
    {
        Vector3 targetPosition = transform.localPosition + direction * moveDistance;

        // Check if path is blocked
        if (Physics.Raycast(transform.localPosition, direction, moveDistance, obstacleLayer))
            return;

        StartCoroutine(MoveBlock(targetPosition));
    }

    IEnumerator MoveBlock(Vector3 target)
    {
        isMoving = true;

        Vector3 start = transform.localPosition;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            transform.localPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.localPosition = target;
        isMoving = false;
    }

    Vector3 GetCardinalDirection(Vector3 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            return dir.x > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            return dir.z > 0 ? Vector3.forward : Vector3.back;
        }
    }
}