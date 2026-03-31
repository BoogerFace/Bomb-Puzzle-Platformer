using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    public Vector3 checkpointPosition;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Respawn();
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
    }

    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}