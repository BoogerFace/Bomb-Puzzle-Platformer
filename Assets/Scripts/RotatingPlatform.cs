using UnityEngine;

public class RotatingPlatform : TriggerableObject
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = new Vector3(0, 0, 1);
    public float rotationSpeed = 100f;

    private bool isRotating = true; // Starts rotating

    void Update()
    {
        if (!isRotating) return;

        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }

    public override void Triggered()
    {
        isRotating = !isRotating; // Toggle
        Debug.Log("Rotating Platform toggled");
    }
}