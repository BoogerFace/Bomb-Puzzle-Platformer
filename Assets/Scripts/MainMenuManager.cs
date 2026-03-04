using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "GameScene";

    [Header("UI References")]
    public GameObject optionsPanel;

    [Header("Audio References")]
    public AudioSource musicSource;

    [Header("Audio Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Slider Sound")]
    public AudioClip sliderTickSound;

    void Start()
    {
        optionsPanel.SetActive(false);

        // Set slider values WITHOUT triggering sound spam
        musicSlider.SetValueWithoutNotify(musicSource.volume);
        sfxSlider.SetValueWithoutNotify(SFXManager.instance.GetVolume());

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
        SFXManager.instance.SetVolume(volume);

        // Play feedback sound at NEW volume
        SFXManager.instance.PlaySound(sliderTickSound);
    }
}