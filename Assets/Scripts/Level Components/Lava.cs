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
    [SerializeField] private GameObject lavaEffect;


    [SerializeField] private float lavaDrag = 5f;
    [SerializeField] private float lavaAngularDrag = 5f;
    [SerializeField] private float fallSlowMultiplier = 0.3f;
    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();
    private Dictionary<Rigidbody, (float drag, float angularDrag)> originalPhysics
    = new Dictionary<Rigidbody, (float, float)>();
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

        Rigidbody rb = other.attachedRigidbody;

        if (rb != null && !originalPhysics.ContainsKey(rb))
        {
            // Store original values
            originalPhysics.Add(rb, (rb.linearDamping, rb.angularDamping));

            // Apply lava physics
            rb.linearDamping = lavaDrag;
            rb.angularDamping = lavaAngularDrag;

            // Slow downward velocity
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                rb.linearVelocity.y * fallSlowMultiplier,
                rb.linearVelocity.z
            );
        }

        if (activeCoroutines.ContainsKey(other.gameObject))
            return;

        float delay = GetDestroyDelay(other);

        Coroutine c = StartCoroutine(DestroyAfterDelay(other.gameObject, delay));
        activeCoroutines.Add(other.gameObject, c);
    }

    private float GetDestroyDelay(Collider other)
    {
        if (other.CompareTag(megaBombTag))
            return destroyDelayBomb;

        if (other.CompareTag(destructibleTag))
            return destroyDelayPlatform;

        return destroyDelayPlatform;
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop destruction coroutine
        if (activeCoroutines.TryGetValue(other.gameObject, out Coroutine c))
        {
            StopCoroutine(c);
            activeCoroutines.Remove(other.gameObject);
        }

        // Restore Rigidbody physics
        Rigidbody rb = other.attachedRigidbody;

        if (rb != null && originalPhysics.TryGetValue(rb, out var original))
        {
            rb.linearDamping = original.drag;
            rb.angularDamping = original.angularDrag;

            originalPhysics.Remove(rb);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isValidTarget(other))
            return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null)
            return;

        // Continuously damp downward velocity
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            Mathf.Max(rb.linearVelocity.y, -2f), // limit fall speed
            rb.linearVelocity.z
        );
    }

    private bool isValidTarget(Collider other)
    {
        return other.CompareTag(megaBombTag) || other.CompareTag(destructibleTag);
    }


    private IEnumerator DestroyAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target != null)
        {
            if (lavaEffect != null)
            {
                Vector3 spawnPos = target.transform.position + Vector3.up * 0.5f;

                GameObject fx = Instantiate(
                    lavaEffect,
                    spawnPos,
                    Quaternion.identity

                );
                Debug.Log("Spawning lava effect at: " + spawnPos);
                Destroy(fx, 2f); 
            }

            Destroy(target);
        }

        activeCoroutines.Remove(target);
    }
}
