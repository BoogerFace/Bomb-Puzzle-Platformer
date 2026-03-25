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

    [Header("Cooldown")]
    [SerializeField] private float cooldown = 0.3f;
    private bool canLaunch = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (!canLaunch)
            return;

        Vector3 direction = transform.TransformDirection(launchDirection.normalized);
        Vector3 origin = transform.position + direction * 0.2f;

        Debug.DrawRay(origin, direction * checkDistance, Color.yellow, 1f);

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, checkDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            Debug.Log("Raycast HIT: " + hitObject.name);

            // 🚧 BLOCK CHECK
            if (IsInLayerMask(hitObject.layer, blockingLayers))
            {
                Debug.Log("❌ BLOCKED");
                SetBounce(false);
                return;
            }

            // 🎯 LAUNCHABLE CHECK
            if (!IsInLayerMask(hitObject.layer, launchableLayers))
            {
                Debug.Log("⚠️ Not launchable");
                SetBounce(false);
                return;
            }

            Rigidbody rb = hitObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                SetBounce(false);
                return;
            }

            // 🚀 LAUNCH
            if (overrideVelocity)
                rb.linearVelocity = direction * launchForce;
            else
                rb.AddForce(direction * launchForce, ForceMode.VelocityChange);

            Debug.Log("✅ Launch + Animation");

            // 🎬 SET BOOL TRUE
            SetBounce(true);

            StartCoroutine(ResetBounce());
            StartCoroutine(CooldownRoutine());
        }
        else
        {
            Debug.Log("❌ Nothing detected");
            SetBounce(false);
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
        yield return new WaitForSeconds(0.2f); // match animation timing
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