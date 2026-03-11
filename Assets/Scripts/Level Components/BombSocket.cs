using UnityEngine;
using UnityEngine.Events;

public class BombSocket : MonoBehaviour
{
    [Header("Socket Settings")]
    [SerializeField] private string bombTag = "MegaBomb";
    [SerializeField] private Transform snapPoint;
    [SerializeField] private bool destroyBombAfterExplosion = true;

    [Header("Socket Group")]
    [SerializeField] private BombSocketGroup socketGroup;

    [Header("Events")]
    public UnityEvent onBombAccepted;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag(bombTag))
            return;

        Rigidbody rb = other.attachedRigidbody;
        MegaBomb bomb = other.GetComponent<MegaBomb>();

        if (bomb == null)
            return;

        triggered = true;

        // Stop bomb movement
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Snap bomb into socket
        if (snapPoint != null)
        {
            other.transform.position = snapPoint.position;
            other.transform.rotation = snapPoint.rotation;
        }

        // Trigger explosion
        bomb.Explode();

        // Local socket event
        onBombAccepted.Invoke();

        // Notify socket group
        if (socketGroup != null)
        {
            socketGroup.RegisterSocketActivation();
        }

        // Optional cleanup
        if (destroyBombAfterExplosion)
        {
            Destroy(other.gameObject, 0.1f);
        }
    }
}