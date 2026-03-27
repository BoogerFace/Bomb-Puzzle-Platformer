using UnityEngine;

public class TargetTrigger : Triggers
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called when hit by bomb
    public override void Trigger()
    {
        print("Target Script Triggered!");
        if (objectToTrigger != null)
        {
            objectToTrigger.Triggered();
        }
    }
}
