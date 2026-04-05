using UnityEngine;

public class DoorOpen : TriggerableObject
{
    public bool isOpened = false;

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
        print("Door Script Triggered!");
        isOpened = !isOpened;

        if (isOpened)
        {
            transform.Rotate(0,90,0);
        }
        else
        {
            transform.Rotate(0,-90,0);
        }
    }
}
