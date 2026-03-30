using UnityEngine;

public class Box : MonoBehaviour
{
    [Header("Broken Version")]
    public GameObject brokenPrefab;

    [Header("Explosion Settings")]
    public float pieceForce = 6f;
    public float pieceRadius = 2f;

    [Header("Cleanup")]
    public float despawnTime = 2f;

    public bool isBig = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        gameObject.tag = "Destructible";

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void Break()
    {
        if (brokenPrefab == null)
            return;

        GameObject broken = Instantiate(brokenPrefab, transform.position, transform.rotation);

        Rigidbody[] pieces = broken.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in pieces)
        {
            rb.AddExplosionForce(pieceForce, transform.position, pieceRadius, 0.2f, ForceMode.Impulse);
        }

        Destroy(broken, despawnTime);

        gameObject.SetActive(false);
    }

    public void ResetObject()
    {
        gameObject.SetActive(true);
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}