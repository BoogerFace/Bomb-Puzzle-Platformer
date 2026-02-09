using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Conveyor Settings")]
    public Vector3 moveDirection = Vector3.right; // Local direction
    public float speed = 2f;

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb == null)
            return;

        // Convert local direction to world direction
        Vector3 worldDirection = transform.TransformDirection(moveDirection.normalized);

        rb.MovePosition(
            rb.position + worldDirection * speed * Time.fixedDeltaTime
        );
    }
}
