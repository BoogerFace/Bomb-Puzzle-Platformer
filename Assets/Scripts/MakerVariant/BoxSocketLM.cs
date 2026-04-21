using UnityEngine;
using UnityEngine.Events;

public class BoxSocketLM : MonoBehaviour
{
    [Header("Socket Settings")]
    [SerializeField] private string boxTag = "PushBox";
    [SerializeField] private Transform snapPoint;
    [SerializeField] private bool lockBoxInPlace = true;

    [Header("Events")]
    public UnityEvent onBoxInserted;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (!other.CompareTag(boxTag))
            return;

        Rigidbody rb = other.attachedRigidbody;

        activated = true;

        // Stop physics movement
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }

        // Snap box into socket
        if (snapPoint != null)
        {
            other.transform.position = snapPoint.position;
            other.transform.rotation = snapPoint.rotation;
        }

        // Lock the box so player cannot move it again
        if (lockBoxInPlace && rb != null)
        {
            rb.isKinematic = true;
        }

        // Trigger puzzle event
        onBoxInserted.Invoke();
    }
}