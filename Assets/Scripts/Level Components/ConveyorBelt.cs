using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Conveyor Settings")]
    public Vector3 moveDirection = Vector3.right;
    public float speed = 2f;

    [Header("Tag Filtering")]
    public string[] allowedTags;

    private void OnCollisionStay(Collision collision)
    {
        if (!HasValidTag(collision.gameObject))
            return;

        Rigidbody rb = collision.rigidbody;
        if (rb == null)
            return;

        Vector3 worldDirection = transform.TransformDirection(moveDirection.normalized);

        rb.MovePosition(
            rb.position + worldDirection * speed * Time.fixedDeltaTime
        );
    }

    private bool HasValidTag(GameObject obj)
    {
        foreach (string tag in allowedTags)
        {
            if (obj.CompareTag(tag))
                return true;
        }
        return false;
    }
}