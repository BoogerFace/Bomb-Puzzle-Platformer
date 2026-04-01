using System.Collections;
using UnityEngine;

public class RespawnObject : MonoBehaviour
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

    void Start()
    {
        if (spawnOnStart)
        {
            Spawn();
        }
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
}