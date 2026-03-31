using UnityEngine;

public class SmallBomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 3f;

    [Header("Effects")]
    public GameObject explosionParticlePrefab;

    [Header("Destruction")]
    public string destructibleTag = "Destructible";
    
    private string targetTag = "Targets";

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

            // Break Destructible Boxes
            if (hit.CompareTag(destructibleTag))
            {
                Box box = hit.GetComponent<Box>();

                if (box != null)
                {
                    if (!box.isBig)
                    {
                        box.Break();   // <-- Use Break instead of Destroy
                    }
                }
            }

            // Trigger Targets
            if (hit.CompareTag(targetTag))
            {
                print("Hit Target");
                TargetTrigger targetScript = hit.GetComponent<TargetTrigger>();
                if (targetScript != null)
                {
                    targetScript.Trigger();
                }
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