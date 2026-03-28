using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string level1 = "Level1";
    public string level2 = "Level2";
    public string level3 = "Level3";
    public string level4 = "Level4";

    [Header("UI References")]
    public GameObject optionsPanel;
    public GameObject levelsPanel;

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
        levelsPanel.SetActive(false);

        // Set slider values WITHOUT triggering sound spam
        musicSlider.SetValueWithoutNotify(musicSource.volume);
        sfxSlider.SetValueWithoutNotify(SFXManager.instance.GetVolume());

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    // PLAY BUTTON (OPEN LEVELS PANEL)
    public void PlayGame()
    {
        levelsPanel.SetActive(true);
    }

    // CLOSE LEVELS PANEL
    public void CloseLevels()
    {
        levelsPanel.SetActive(false);
    }

    // LEVEL BUTTONS
    public void LoadLevel1()
    {
        SceneManager.LoadSceneAsync(level1);
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene(level2);
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene(level3);
    }

    public void LoadLevel4()
    {
        SceneManager.LoadScene(level4);
    }

    // STOP MENU MUSIC
    void StopMenuMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
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
    }

    public void PlaySliderReleaseSound()
    {
        SFXManager.instance.PlaySound(sliderTickSound);
    }
}