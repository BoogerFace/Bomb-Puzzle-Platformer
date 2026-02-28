using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "GameScene";   // Name of your game scene

    [Header("UI References")]
    public GameObject optionsPanel;

    [Header("Audio References")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Hide options panel at start
        optionsPanel.SetActive(false);

        // Set slider default values
        musicSlider.value = musicSource.volume;
        sfxSlider.value = sfxSource.volume;

        // Add listeners
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    // PLAY BUTTON
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // OPTIONS BUTTON
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    // CLOSE OPTIONS
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    // QUIT BUTTON
    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }

    // MUSIC VOLUME
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    // SFX VOLUME
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}