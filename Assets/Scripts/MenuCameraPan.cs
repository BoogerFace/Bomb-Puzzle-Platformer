using UnityEngine;

public class MenuCameraPan : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 moveDirection = new Vector3(0.2f, 0f, 0f);
    public float moveSpeed = 0.5f;

    [Header("Rotation (optional)")]
    public Vector3 rotationSpeed = new Vector3(0f, 2f, 0f);

    [Header("Loop Settings")]
    public float resetDistance = 20f;

    private Vector3 startPosition;

    void Start()
    {
        // Store starting position
        startPosition = transform.position;

        // Freeze gameplay but allow camera movement
        Time.timeScale = 0f;
    }

    void Update()
    {
        // Move camera (unaffected by pause)
        transform.Translate(moveDirection * moveSpeed * Time.unscaledDeltaTime, Space.World);

        // Optional slow rotation
        transform.Rotate(rotationSpeed * Time.unscaledDeltaTime);

        // Reset position when too far (loop effect)
        if (Vector3.Distance(transform.position, startPosition) >= resetDistance)
        {
            transform.position = startPosition;
        }
    }
}