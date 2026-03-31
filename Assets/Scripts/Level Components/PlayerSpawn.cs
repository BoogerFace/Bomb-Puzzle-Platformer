using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        if (RespawnManager.instance != null)
        {
            transform.position = RespawnManager.instance.checkpointPosition;
        }
    }
}