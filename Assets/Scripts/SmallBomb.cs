using UnityEngine;

public class SmallBomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 3f;

    [Header("Effects")]
    public GameObject explosionParticlePrefab;

    [Header("Destruction")]
    public string destructibleTag = "SmallDestructible";

    private bool exploded = false;

    public void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        Debug.Log("Small bomb exploded!");

        if (explosionParticlePrefab != null)
        {
            GameObject effect = Instantiate(
                explosionParticlePrefab,
                transform.position,
                Quaternion.identity
            );

            effect.transform.localScale = Vector3.one * explosionRadius;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            Debug.Log("Hit: " + hit.gameObject.name + " Tag: " + hit.gameObject.tag);

            if (hit.CompareTag(destructibleTag))
            {
                Destroy(hit.gameObject);
            }

            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                rb.AddForce(dir * 8f, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }
}