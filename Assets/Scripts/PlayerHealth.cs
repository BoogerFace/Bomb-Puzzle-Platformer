using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private int maxHealth = 5;
    public int health = 5;
    
    private PlayerController playerScript;
    
    [SerializeField] private Image hpDisplay;
    [SerializeField] private Sprite hp1;
    [SerializeField] private Sprite hp2;
    [SerializeField] private Sprite hp3;
    [SerializeField] private Sprite hp4;
    [SerializeField] private Sprite hp5;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // DontDestroyOnLoad(gameObject);
        health = maxHealth;
        playerScript = gameObject.GetComponent<PlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        // if (Keyboard.current.rKey.wasPressedThisFrame)
        // {
        //     TakeDamage(5);
        // }

        if (transform.position.y <= -50 && health > 0)
        {
            TakeDamage(5);
        }
    }


    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            hpDisplay.enabled = false;
            Die();
        }
        else
        {
            hpDisplay.enabled = true;
        }

        switch (health)
        {
            case 5:
                hpDisplay.sprite = hp5;
                break;
            case 4:
                hpDisplay.sprite = hp4;
                break;
            case 3:
                hpDisplay.sprite = hp3;
                break;
            case 2:
                hpDisplay.sprite = hp2;
                break;
            case 1:
                hpDisplay.sprite = hp1;
                break;
            case 0:
                hpDisplay.sprite = null;
                break;
            default:
                hpDisplay.sprite = null;
                break;
        }
    }


    public void Die()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        playerScript.Die();
    }


    public void LavaDie()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        playerScript.LavaDie();
    }
}
