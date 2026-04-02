using UnityEngine;

public class AnimationCalls : MonoBehaviour
{
    public GameObject parent;

    private PlayerController script;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        script = parent.GetComponent<PlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    private void StopStartThrow()
    {
        script.StopStartThrow();
    }


    private void StopMidThrow()
    {
        script.StopMidThrow();
    }


    private void ResetThrow()
    {
        script.ResetThrow();
    }


    private void ThrowBomb()
    {
        script.ThrowBomb();
    }


    private void Reset()
    {
        script.Reset();
    }


    private void EndSpawn()
    {
        script.EndSpawn();
    }
}
