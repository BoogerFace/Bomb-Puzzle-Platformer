using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour
{
    [Header("Spring Settings")]
    [SerializeField] private float launchForce = 15f;
    [SerializeField] private Vector3 launchDirection = Vector3.up;
    [SerializeField] private bool overrideVelocity = true;

    [Header("Detection")]
    [SerializeField] private float checkDistance = 1.2f;
    [SerializeField] private LayerMask launchableLayers;
    [SerializeField] private LayerMask blockingLayers;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string boolName = "IsBouncing";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip springSound;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    [Header("Cooldown")]
    [SerializeField] private float cooldown = 0.3f;
    private bool canLaunch = true;

    void Update()
    {
        if (!canLaunch)
            return;

        CheckForLaunchable();
    }

    void CheckForLaunchable()
    {
        Vector3 direction = transform.TransformDirection(launchDirection.normalized);
        Vector3 origin = transform.position + direction * 0.2f;

        Debug.DrawRay(origin, direction * checkDistance, Color.yellow);

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, checkDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            // 🚧 BLOCKED
            if (IsInLayerMask(hitObject.layer, blockingLayers))
            {
                SetBounce(false);
                return;
            }

            // 🎯 VALID TARGET
            if (IsInLayerMask(hitObject.layer, launchableLayers))
            {
                SetBounce(true);
            }
            else
            {
                SetBounce(false);
            }
        }
        else
        {
            SetBounce(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canLaunch)
            return;

        if (!IsInLayerMask(collision.gameObject.layer, launchableLayers))
            return;

        Rigidbody rb = collision.rigidbody;
        if (rb == null)
            return;

        Vector3 direction = transform.TransformDirection(launchDirection.normalized);

        // 🚀 APPLY LAUNCH
        if (overrideVelocity)
            rb.linearVelocity = direction * launchForce;
        else
            rb.AddForce(direction * launchForce, ForceMode.VelocityChange);

        // 🎬 PLAY ANIMATION
        SetBounce(true);
        StartCoroutine(ResetBounce());

        // 🔊 PLAY SOUND (with improvements)
        PlaySpringSound();

        StartCoroutine(CooldownRoutine());
    }

    private void PlaySpringSound()
    {
        if (audioSource != null && springSound != null)
        {
            // 🎵 Prevent sound spam
            if (audioSource.isPlaying)
                return;

            // 🎵 Random pitch variation
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);

            audioSource.PlayOneShot(springSound);
        }
    }

    private void SetBounce(bool state)
    {
        if (animator != null)
        {
            animator.SetBool(boolName, state);
        }
    }

    private IEnumerator ResetBounce()
    {
        yield return new WaitForSeconds(0.2f);
        SetBounce(false);
    }

    private IEnumerator CooldownRoutine()
    {
        canLaunch = false;
        yield return new WaitForSeconds(cooldown);
        canLaunch = true;
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    private void OnDrawGizmos()
    {
        Vector3 direction = transform.TransformDirection(launchDirection.normalized);
        Vector3 origin = transform.position + direction * 0.2f;

        Gizmos.color = Color.yellow;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, checkDistance))
        {
            if (IsInLayerMask(hit.collider.gameObject.layer, blockingLayers))
                Gizmos.color = Color.red;
            else if (IsInLayerMask(hit.collider.gameObject.layer, launchableLayers))
                Gizmos.color = Color.green;
        }

        Gizmos.DrawLine(origin, origin + direction * checkDistance);
    }
}