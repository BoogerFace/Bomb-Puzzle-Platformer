using UnityEngine;

public class SceneDestructible : MonoBehaviour
{
    public GameObject brokenPrefab;
    public float pieceForce = 6f;
    public float pieceRadius = 2f;
    public float despawnTime = 2f;

    private void Awake()
    {
        gameObject.tag = "Destructible";
    }

    // Call this to destroy manually
    public void DestroyDestructable()
    {
        if (brokenPrefab != null)
        {
            GameObject broken = Instantiate(brokenPrefab, transform.position, transform.rotation);
            Rigidbody[] pieces = broken.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in pieces)
            {
                rb.AddExplosionForce(pieceForce, transform.position, pieceRadius, 0.2f, ForceMode.Impulse);
            }
            Destroy(broken, despawnTime);
        }

        Destroy(gameObject);
    }

    // Keep OnDestroy for any other automatic destruction
    private void OnDestroy()
    {
        if (!Application.isPlaying)
            return;

        // Only run if DestroyDestructable wasn't called manually
        // (Optional: leave empty if using DestroyDestructable for all cases)
    }
}