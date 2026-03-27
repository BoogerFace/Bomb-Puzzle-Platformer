using UnityEngine;
using UnityEngine.InputSystem;

public class MegaBomb : TriggerableObject
{
    [Header("Explosion Settings")]
    public float explosionRadius = 3f;

    [Header("Effects")]
    public GameObject explosionParticlePrefab;

    [Header("Destruction")]
    public string destructibleTag = "Destructible";

    private bool exploded = false;

    void Update()
    {
        // Press B to detonate
        if (Keyboard.current.bKey.wasPressedThisFrame && !exploded)
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        // Spawn particle effect
        if (explosionParticlePrefab != null)
        {
            GameObject effect = Instantiate(
                explosionParticlePrefab,
                transform.position,
                Quaternion.identity
            );

            effect.transform.localScale = Vector3.one * explosionRadius;
        }

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            explosionRadius
        );

        foreach (Collider hit in hits)
        {
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // Called when a Trigger is activated
    public override void Triggered()
    {
        Explode();
        print("Mega Bomb Script Triggered!");
    }
}