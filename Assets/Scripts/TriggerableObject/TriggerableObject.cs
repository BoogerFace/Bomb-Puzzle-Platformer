using UnityEngine;

public class TriggerableObject : MonoBehaviour
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
    public virtual void Triggered()
    {
        print("Object was triggered!");
    }
}
