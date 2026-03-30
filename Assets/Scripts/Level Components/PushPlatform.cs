using UnityEngine;
using System.Collections;

public class PushPlatform : MonoBehaviour
{
    [Header("Push Settings")]
    public float moveDistance = 2f;
    public float moveSpeed = 4f;
    public LayerMask obstacleLayer;

    [Header("Push Detection")]
    [SerializeField] private float topOffsetTolerance = 0.1f;
    [SerializeField] private float minPushVelocity = 0.1f;

    private bool isMoving = false;
    private Collider platformCollider;

    void Start()
    {
        platformCollider = GetComponent<Collider>();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isMoving)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        Transform player = collision.transform;

        // 🧠 HEIGHT CHECK (still use WORLD space for this)
        float playerY = player.position.y;
        float platformTop = platformCollider.bounds.max.y;

        if (playerY > platformTop - topOffsetTolerance)
        {
            Debug.Log("Player standing on top — no push");
            return;
        }

        // 🧠 Require player movement
        Rigidbody playerRb = collision.rigidbody;
        if (playerRb != null && playerRb.linearVelocity.magnitude < minPushVelocity)
        {
            return;
        }

        // ✅ Convert player position into LOCAL SPACE
        Vector3 localPlayerPos = transform.parent != null
            ? transform.parent.InverseTransformPoint(player.position)
            : player.position;

        Vector3 pushDirection = transform.localPosition - localPlayerPos;

        pushDirection.y = 0;
        pushDirection.Normalize();

        Vector3 moveDir = GetCardinalDirection(pushDirection);

        TryMove(moveDir);
    }

    void TryMove(Vector3 direction)
    {
        Vector3 targetPosition = transform.localPosition + direction * moveDistance;

        // 🚧 Raycast MUST be in WORLD space
        Vector3 worldDir = transform.parent != null
            ? transform.parent.TransformDirection(direction)
            : direction;

        if (Physics.Raycast(transform.position, worldDir, moveDistance, obstacleLayer))
        {
            Debug.Log("Blocked — cannot move");
            return;
        }

        StartCoroutine(MovePlatform(targetPosition));
    }

    IEnumerator MovePlatform(Vector3 target)
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