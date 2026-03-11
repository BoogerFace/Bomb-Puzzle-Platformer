using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float destroyDelayBomb = 1.5f;
    [SerializeField] private float destroyDelayPlatform = 1.5f;
    [SerializeField] private string megaBombTag = "MegaBomb";
    [SerializeField] private string destructibleTag = "Destructible";

    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();

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
        if (!isValidTarget(other))
            return;

        if (activeCoroutines.ContainsKey(other.gameObject)) //Multiple destruction timers on the same object and Double - destroy bugs
            return;

        Coroutine c = StartCoroutine(DestroyAfterDelay(other.gameObject)); //Starts a coroutine that waits
        activeCoroutines.Add(other.gameObject, c); //Stores a reference so we can stop it later
    }


    private void OnTriggerExit(Collider other)
    {
        if (activeCoroutines.TryGetValue(other.gameObject, out Coroutine c))
        {
            StopCoroutine(c); //stops the countdown
            activeCoroutines.Remove(other.gameObject); //removes object from tracking
        }
    }

    private bool isValidTarget(Collider other)
    {
        return other.CompareTag(megaBombTag) || other.CompareTag(destructibleTag);
    }


    private IEnumerator DestroyAfterDelay(GameObject target)
    {
        yield return new WaitForSeconds(destroyDelayPlatform);

        if (target != null)
        {
            Destroy(target);
        }

        activeCoroutines.Remove(target);
    }

}
