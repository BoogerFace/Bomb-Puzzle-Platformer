using UnityEngine;

public class SmallDestructable : MonoBehaviour
{
    public GameObject brokenPrefab;
    public float pieceForce = 6f;
    public float pieceRadius = 2f;
    public float despawnTime = 2f;

    private void Awake()
    {
        gameObject.tag = "Destructible";
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying)
            return;

        if (brokenPrefab == null)
            return;

        GameObject broken = Instantiate(brokenPrefab, transform.position, transform.rotation);

        Rigidbody[] pieces = broken.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in pieces)
        {
            rb.AddExplosionForce(pieceForce, transform.position, pieceRadius, 0.2f, ForceMode.Impulse);
        }

        Destroy(broken, despawnTime);
    }
}