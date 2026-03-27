using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class BombSocketGroup : MonoBehaviour
{
    [Header("Socket Requirements")]
    [SerializeField] private int requiredSockets = 2;

    [Header("Destructibles to Destroy")]
    [SerializeField] private List<SceneDestructible> destructibles;

    [Header("Events")]
    public UnityEvent onAllSocketsActivated;

    private int currentActivated = 0;
    private bool triggered = false;

    public void RegisterSocketActivation()
    {
        if (triggered)
            return;

        currentActivated++;
        Debug.Log("Bomb socket activated: " + currentActivated + "/" + requiredSockets);

        if (currentActivated >= requiredSockets)
        {
            triggered = true;
            Debug.Log("All bomb sockets activated!");

            // Destroy selected destructibles
            foreach (var obj in destructibles)
            {
                if (obj != null)
                    obj.DestroyDestructable();
            }

            // Fire any additional UnityEvent
            onAllSocketsActivated.Invoke();
        }
    }
}