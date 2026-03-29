using UnityEngine;

public class DisableObstacle : Triggers
{
    public GameObject objectToDisable;

    public override void Trigger()
    {
        Debug.Log("Obstacle Disabled!");

        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }
    }
}