using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Flag : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject messageUI; // Assign your text UI (e.g. "Level Complete!")

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip flagSound;

    [Header("VFX")]
    [SerializeField] private GameObject flagEffect;

    [Header("Scene Transition")]
    [SerializeField] private float delayBeforeLoad = 3f;
    [SerializeField] private string nextSceneName;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        Debug.Log("🏁 Flag triggered!");

        // ✅ Show UI
        if (messageUI != null)
            messageUI.SetActive(true);

        // ✅ Play sound
        if (audioSource != null && flagSound != null)
            audioSource.PlayOneShot(flagSound);

        // ✅ Spawn particle effect
        if (flagEffect != null)
        {
            GameObject fx = Instantiate(
                flagEffect,
                transform.position,
                Quaternion.identity
            );

            Destroy(fx, 3f);
        }

        // ✅ Load next level
        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(delayBeforeLoad);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("No next scene assigned!");
        }
    }
}