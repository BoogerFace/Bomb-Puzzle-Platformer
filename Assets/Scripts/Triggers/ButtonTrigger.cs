using UnityEngine;

public class ButtonTrigger : Triggers
{
    [SerializeField] private GameObject buttonModel;

    private bool isPressed = false;
    private int objectsOnTop = 0;
    private float buttonMovement = .1f;

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
        objectsOnTop += 1;
        
        if (!isPressed)
        {
            isPressed = true;
            buttonModel.transform.Translate(new Vector3(0, -buttonMovement, 0));
            Trigger();
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        objectsOnTop -= 1;
        
        if (isPressed && objectsOnTop <= 0)
        {
            isPressed = false;
            buttonModel.transform.Translate(new Vector3(0, buttonMovement, 0));
        }
    }


    public override void Trigger()
    {
        print("Button Script Triggered!");
        if (objectToTrigger != null)
        {
            objectToTrigger.Triggered();
        }
    }
}
