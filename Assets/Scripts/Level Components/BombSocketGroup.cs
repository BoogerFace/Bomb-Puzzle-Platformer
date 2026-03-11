using UnityEngine;
using UnityEngine.Events;

public class BombSocketGroup : MonoBehaviour
{
    [Header("Socket Requirements")]
    [SerializeField] private int requiredSockets = 2;

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

            onAllSocketsActivated.Invoke();
        }
    }
}