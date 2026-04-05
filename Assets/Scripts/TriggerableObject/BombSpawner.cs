using UnityEngine;

public class BombSpawner : TriggerableObject
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject bombSpawnPoint;
    [SerializeField] private float bombRadius = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called when activated by a Trigger
    public override void Triggered()
    {
        print("Bomb Spawner Script Triggered!");
        GameObject bomb = Instantiate(bombPrefab, bombSpawnPoint.transform.position, bombPrefab.transform.rotation);
        
        MegaBomb bombScript = bomb.GetComponent<MegaBomb>();
        if (bombScript != null)
        {
            bombScript.explosionRadius = bombRadius;
        }
    }
}
