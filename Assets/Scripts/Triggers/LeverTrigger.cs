using UnityEngine;

public class LeverTrigger : Triggers
{
    private string playerTag = "Player";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        GameObject playerobject = other.transform.parent.gameObject;
        if (playerobject.CompareTag(playerTag))
        {
            PlayerMove playerScript = playerobject.GetComponent<PlayerMove>();
            if (playerScript != null)
            {
                playerScript.canInteract = true;
                playerScript.currentInteractable = transform.gameObject;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        GameObject playerobject = other.transform.parent.gameObject;
        if (playerobject.CompareTag(playerTag))
        {
            PlayerMove playerScript = playerobject.GetComponent<PlayerMove>();
            if (playerScript != null)
            {
                playerScript.canInteract = false;
                playerScript.currentInteractable = null;
            }
        }
    }


    // Called when interacted
    public override void Trigger()
    {
        print("Lever Script Triggered!");
        if (objectToTrigger != null)
        {
            objectToTrigger.Triggered();
        }
    }
}
