using UnityEngine;

public class Spring : MonoBehaviour
{
    [Header("Spring Force")]
    public float launchForce = 20f;
    public Vector2 launchDirection = Vector2.up;

    [Header("Target Filtering")]
    public LayerMask affectedLayers;

    [Header("Optional")]
    public Animator animator;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if object is in allowed layers
        if ((affectedLayers.value & (1 << collision.gameObject.layer)) == 0)
            return;

        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb == null) return;

        // Normalize direction just in case
        Vector2 dir = launchDirection.normalized;

        // Reset velocity for consistent bounce
        rb.linearVelocity = Vector2.zero;

        // Apply force
        rb.AddForce(dir * launchForce, ForceMode2D.Impulse);

        // Optional: trigger animation
        if (animator != null)
        {
            animator.SetTrigger("Bounce");
        }
    }
}