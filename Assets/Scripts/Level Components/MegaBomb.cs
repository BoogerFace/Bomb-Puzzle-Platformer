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

    private string playerTag = "Player";
    private float knockbackForce = 40f;

    private int bigBombDmg = 3;


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
                Box box = hit.GetComponent<Box>();
                if (box != null)
                {
                    box.Break();
                }
            }

            // Player Knockback for bomb jump
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null && hit.CompareTag(playerTag))
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
                GameObject playerobject = hit.transform.parent.gameObject;
                PlayerController playerScript = playerobject.GetComponent<PlayerController>();
                PlayerHealth playerHealth = playerobject.GetComponent<PlayerHealth>();
                if (playerScript != null)
                {
                    playerScript.isBombJumping = true;
                    playerScript.bombForce = dir * knockbackForce;
                    playerScript.Invoke(nameof(playerScript.ResetBombJump), .5f);
                }
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(bigBombDmg);
                }
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null)
        {
            GameObject playerobject = other.transform.parent.gameObject;
            if (playerobject.CompareTag(playerTag))
            {
                PlayerController playerScript = playerobject.GetComponent<PlayerController>();
                if (playerScript != null)
                {
                    playerScript.canInteract = true;
                    playerScript.currentInteractable = transform.gameObject;
                }
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent != null)
        {
            GameObject playerobject = other.transform.parent.gameObject;
            if (playerobject.CompareTag(playerTag))
            {
                PlayerController playerScript = playerobject.GetComponent<PlayerController>();
                if (playerScript != null)
                {
                    playerScript.canInteract = false;
                    playerScript.currentInteractable = null;
                }
            }
        }
    }
}