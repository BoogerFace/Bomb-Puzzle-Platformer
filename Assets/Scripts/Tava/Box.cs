using UnityEngine;

public class Box : MonoBehaviour
{
    public GameObject brokenPrefab;
    public float pieceForce = 6f;
    public float pieceRadius = 2f;
    public float despawnTime = 2f;

    void Awake()
    {
        gameObject.tag = "Destructible";
    }

    void OnDestroy()
    {
        if (!Application.isPlaying)
            return;

        if (brokenPrefab == null)
            return;

        GameObject broken = Instantiate(brokenPrefab, transform.position, transform.rotation);

        Rigidbody[] pieces = broken.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in pieces)
        {
            rb.AddExplosionForce(pieceForce, transform.position, pieceRadius);
        }

        Destroy(broken, despawnTime);
    }
}