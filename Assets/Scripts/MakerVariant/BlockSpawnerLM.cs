using System.Collections;
using UnityEngine;
using static System.Enum;

public class BlockSpawnerLM : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private float respawnDelay = 3f;
    [SerializeField] private bool spawnOnStart = true;
    public GameObject spawnParent;

    [Header("Optional")]
    [SerializeField] private Transform spawnPoint;

    private GameObject currentInstance;
    private bool respawning = false;
    
    enum Direction
    {
        Forward,
        Left,
        Back,
        Right
    }
    [SerializeField] private Direction blockDirection = Direction.Forward;
    [SerializeField] private GameObject[] blockPrefabs;
    [SerializeField] private GameObject blockDisplay;


    void Start()
    {
        spawnParent = GameObject.Find("World");
        blockDisplay.SetActive(true);
    }


    void Spawn()
    {
        if (currentInstance != null)
            return;

        Transform point = spawnPoint != null ? spawnPoint : transform;

        currentInstance = Instantiate(
            prefabToSpawn,
            point.position,
            point.rotation
        );

        currentInstance.transform.SetParent(
            spawnParent != null ? spawnParent.transform : null
        );

        respawning = false;
    }


    void Update()
    {
        if (currentInstance == null && !respawning)
        {
            StartCoroutine(RespawnRoutine());
        }
    }


    IEnumerator RespawnRoutine()
    {
        respawning = true;
        yield return new WaitForSeconds(respawnDelay);
        Spawn();
    }


    public void OnLevelStart()
    {
        blockDisplay.SetActive(false);
        if (spawnOnStart)
        {
            Spawn();
        }
    }


    public void OnChangeDirection(int value)
    {
        blockDirection += value;

        blockDirection = (Direction) ((int)blockDirection % GetValues(typeof(Direction)).Length);
        if (blockDirection < 0) blockDirection += GetValues(typeof(Direction)).Length;

        blockDisplay.transform.Rotate(0f, -90f*value, 0f, Space.Self);
    }
}
