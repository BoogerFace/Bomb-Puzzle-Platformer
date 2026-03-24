using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour
{
    [Header("Spring Settings")]
    [SerializeField] private float launchForce = 15f;
    [SerializeField] private Vector3 launchDirection = Vector3.up;
    [SerializeField] private bool overrideVelocity = true;

    [Header("Layer Filtering")]
    [SerializeField] private LayerMask launchableLayers;

    [Header("Blocking Check")]
    [SerializeField] private float checkDistance = 1.2f;
    [SerializeField] private LayerMask blockingLayers;

    [Header("Cooldown")]
    [SerializeField] private float cooldown = 0.3f;
    private bool canLaunch = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (!canLaunch)
            return;

        // ✅ Only affect allowed layers (e.g. MegaBomb)
        if (!IsInLayerMask(collision.gameObject.layer, launchableLayers))
            return;

        Rigidbody rb = collision.rigidbody;
        if (rb == null)
            return;

        Vector3 direction = transform.TransformDirection(launchDirection.normalized);

        // ✅ Check if something is blocking immediately in front
        if (Physics.Raycast(transform.position, direction, checkDistance, blockingLayers))
        {
            Debug.Log("Spring blocked — not launching");
            return;
        }

        // ✅ Apply launch
        if (overrideVelocity)
        {
            rb.linearVelocity = direction * launchForce;
        }
        else
        {
            rb.AddForce(direction * launchForce, ForceMode.VelocityChange);
        }

        // ✅ Start cooldown
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        canLaunch = false;
        yield return new WaitForSeconds(cooldown);
        canLaunch = true;
    }

    // ✅ Layer check helper
    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    // ✅ Debug visualization in Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 direction = transform.TransformDirection(launchDirection.normalized);
        Gizmos.DrawLine(transform.position, transform.position + direction * checkDistance);
    }
}