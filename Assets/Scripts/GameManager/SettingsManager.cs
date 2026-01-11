using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Needed for the Back button

public class SettingsMenu : MonoBehaviour
{
    //[Header("Audio Settings")]
    //public AudioMixer mainMixer;    // Drag your AudioMixer here
    public Slider volumeSlider;     // Drag your Volume Slider here
    public static string previousSceneName;
    [Header("Display Settings")]
    public TMP_Dropdown resolutionDropdown; // Drag your Resolution Dropdown here
    public Toggle fullscreenToggle;         // Drag your Full Screen Toggle here
    private bool isInitializing = true;
    private Resolution[] resolutions;

    void Start()
    {
        // 1. Enable the flag so we don't trigger events while setting up
        isInitializing = true;

        // --- SETUP RESOLUTIONS ---
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        //// Load saved width/height (more reliable than index)
        //int savedWidth = PlayerPrefs.GetInt("ResWidth", Screen.currentResolution.width);
        //int savedHeight = PlayerPrefs.GetInt("ResHeight", Screen.currentResolution.height);

        int savedWidth = GameDataManager.Instance.gameData.resolutionWidth;
        int savedHeight = GameDataManager.Instance.gameData.resolutionHeight;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Check if this resolution matches what we have saved
            if (resolutions[i].width == savedWidth && resolutions[i].height == savedHeight)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // --- SETUP VOLUME ---
        // Load volume, default to 1 (Max)
        //float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float savedVolume = GameDataManager.Instance.gameData.volume;
        volumeSlider.value = savedVolume;
        // Force apply it now so the game sounds right immediately
        //mainMixer.SetFloat("MasterVolume", Mathf.Log10(savedVolume) * 20);

        // --- SETUP FULLSCREEN ---
        //bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        bool isFullscreen = GameDataManager.Instance.gameData.fullscreen;
        fullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;

        // 2. Setup is done. Disable the flag. Now events can run.
        isInitializing = false;
    }

    // --- UI EVENTS ---

    public void SetVolume(float volume)
    {
        // Logarithmic conversion for natural sound fading
        //mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        // Save automatically so it remembers even if they don't hit "Save"
        //PlayerPrefs.SetFloat("MasterVolume", volume);
        Debug.Log(volume);
        GameDataManager.Instance.gameData.volume = volume;
        Debug.Log(GameDataManager.Instance.gameData.volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        //PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        GameDataManager.Instance.gameData.fullscreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Debug.Log(resolutionIndex);

        Resolution resolution = resolutions[resolutionIndex];


        //Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        GameDataManager.Instance.gameData.resolutionHeight = resolution.height;
        GameDataManager.Instance.gameData.resolutionWidth = resolution.width;
    }

    // --- BUTTONS ---

    public void OnSaveButton()
    {
        Debug.Log(volumeSlider.value);
        Debug.Log(fullscreenToggle.isOn);
        Debug.Log(resolutionDropdown.value);
        GameDataManager.Instance.gameData.volume = volumeSlider.value;
        GameDataManager.Instance.gameData.fullscreen = fullscreenToggle.isOn;
        GameDataManager.Instance.gameData.resolutionWidth = resolutions[resolutionDropdown.value].width;
        GameDataManager.Instance.gameData.resolutionHeight = resolutions[resolutionDropdown.value].height;

        SoundFxManager.instance.effectVolume = volumeSlider.value;
        //Screen.fullScreen = fullscreenToggle.isOn;
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, fullscreenToggle.isOn);
        GameDataManager.Instance.SaveGame();
    }

    public void OnBackButton()
    {
        // 2. Check where we came from and decide what to do
        if (previousSceneName == "Main Menu")
        {
            // If we came from the menu, just reload the Main Menu scene
            SceneManager.LoadScene("Main Menu");
        }
        else if (string.IsNullOrEmpty(previousSceneName))
        {
            // Just unload the settings scene, revealing the frozen game behind it
            Debug.Log("No scene found - Defaulting to Main Menu");
            SceneManager.LoadScene("Main Menu");
        }
        else
        {
            SceneManager.UnloadSceneAsync("Settings");
           
        }
    }
}