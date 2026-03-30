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

    private string playerTag = "Player";
    private float knockbackForce = 20f;


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

            // Break Destructible
            if (hit.CompareTag(destructibleTag))
            {
                if (!hit.gameObject.GetComponent<Box>().isBig)
                {
                    Destroy(hit.gameObject);
                }
            }

            // Knockback for bomb jump
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null && hit.CompareTag(playerTag))
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
                GameObject playerobject = hit.transform.parent.gameObject;
                PlayerMove playerScript = playerobject.GetComponent<PlayerMove>();
                if (playerScript != null)
                {
                    playerScript.isBombJumping = true;
                    playerScript.bombForce = dir * knockbackForce;
                    playerScript.Invoke(nameof(playerScript.ResetBombJump), .5f);
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