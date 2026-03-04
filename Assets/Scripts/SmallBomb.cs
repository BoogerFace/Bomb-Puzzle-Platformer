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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
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

    
    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }
}
