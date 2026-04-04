using UnityEngine;
using System.Collections;


public class BombPickup : MonoBehaviour
{
    [Range(1, 4)]
    public int maxBombCount = 1;
    public bool canReplenish;
    public GameObject[] bombModels;

    private int currentBombCount = 1;
    private bool isReplenishing = false;
    public float replenishTimer = 15;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentBombCount = maxBombCount;

        for (int i = 0; i < bombModels.Length; i++)
        {
            bombModels[i].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject playerobject = other.transform.parent.gameObject;
            PlayerController playerScript = playerobject.GetComponent<PlayerController>();
            if (playerScript != null && currentBombCount > 0)
            {
                playerScript.ammo += currentBombCount;
                playerScript.UpdateAmmoDisplay();
                currentBombCount = 0;
                for (int i = 0; i < bombModels.Length; i++)
                {
                    bombModels[i].SetActive(false);
                }

                if (canReplenish && !isReplenishing)
                {
                    isReplenishing = true;
                    StartCoroutine(ReplenishBombAfterTime(replenishTimer));
                }
            }
        }
    }

    IEnumerator ReplenishBombAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        if (currentBombCount < maxBombCount)
        {
            bombModels[currentBombCount].SetActive(true);
            currentBombCount += 1;
            StartCoroutine(ReplenishBombAfterTime(replenishTimer));
        }
        else
        {
            isReplenishing = false;
        }
    }
}
