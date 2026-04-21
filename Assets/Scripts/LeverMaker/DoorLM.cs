using UnityEngine;

public class DoorLM : TriggerableObject
{
    public bool isOpened = false;
    [SerializeField] private GameObject model;

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
            model.transform.Rotate(0,-90,0);
        }
        else
        {
            model.transform.Rotate(0,90,0);
        }
    }
}
