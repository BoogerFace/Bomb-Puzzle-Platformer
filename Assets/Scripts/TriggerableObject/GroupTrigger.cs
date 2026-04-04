using UnityEngine;

public class GroupTrigger : TriggerableObject
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called when activated by a Trigger
    public override void Triggered()
    {
        print("Group Script Triggered!");

        foreach (Transform child in transform)
        {
            TriggerableObject childScript = child.gameObject.GetComponent<TriggerableObject>();

            if (childScript != null)
            {
                childScript.Triggered();
            }
        }
    }
}
