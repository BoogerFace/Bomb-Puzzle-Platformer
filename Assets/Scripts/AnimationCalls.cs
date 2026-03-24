using UnityEngine;

public class AnimationCalls : MonoBehaviour
{
    public GameObject parent;

    private PlayerMove script;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        script = parent.GetComponent<PlayerMove>();
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
}
