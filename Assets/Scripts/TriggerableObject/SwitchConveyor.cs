using UnityEngine;

public class SwitchConveyor : TriggerableObject
{
    private ConveyorBelt selfConveyorScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selfConveyorScript = transform.gameObject.GetComponent<ConveyorBelt>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called when activated by a Trigger
    public override void Triggered()
    {
        print("Switch Conveyor Script Triggered!");
        transform.Rotate(0,-90,0);
    }
}
