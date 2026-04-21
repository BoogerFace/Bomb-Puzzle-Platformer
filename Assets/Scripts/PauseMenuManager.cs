using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager instance;

    [Header("UI References")]
    public GameObject pausePanel;

    [Header("Audio Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Buttons")]
    public Button resumeButton;
    public Button mainMenuButton;

    [Header("Audio References")]
    public AudioSource musicSource;

    [Header("Slider Sound")]
    public AudioClip sliderTickSound;

    private bool isPaused = false;
    
    [Header("Level Maker Reset Config")]
    [SerializeField] private bool isLevelMaker = false;
    [SerializeField] private GameObject makerController;
    [SerializeField] private GameObject makerCamPvot;
    [SerializeField] private GameObject makerUI;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject fakePlayer;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Initialize sliders safely
        if (musicSlider != null && musicSource != null)
            musicSlider.SetValueWithoutNotify(musicSource.volume);

        if (sfxSlider != null && SFXManager.instance != null)
            sfxSlider.SetValueWithoutNotify(SFXManager.instance.GetVolume());

        // Add listeners for slider value changes
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // Assign button actions
        if (resumeButton != null)
            resumeButton.onClick.AddListener(TogglePause);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    void Update()
    {
        // Toggle pause with Escape key
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isLevelMaker && makerController.GetComponent<MakerController>().isPlaying)
            {
                ResetLevelMaker();
            }
            else
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (SFXManager.instance != null)
            SFXManager.instance.SetVolume(volume);
    }

    public void PlaySliderTick()
    {
        if (sliderTickSound != null && SFXManager.instance != null)
            SFXManager.instance.PlaySound(sliderTickSound);
    }

    void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Unpause
        SceneManager.LoadScene("MainMenu"); // Change if needed
    }

    private void ResetLevelMaker()
    {
        player.SetActive(false);
        playerUI.SetActive(false);

        fakePlayer.SetActive(true);
        makerController.gameObject.SetActive(true);
        makerUI.gameObject.SetActive(true);
        makerCamPvot.gameObject.SetActive(true);

        InputSystem.actions.FindActionMap("LevelMaker").Enable();
        makerController.GetComponent<MakerController>().isPlaying = false;
    }
}