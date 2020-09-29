using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;

    public Slider[] sliders;
    public Toggle[] resolutionToggles;
    public int[] screenWidths;
    public Toggle fullsceenToggle;

    public event Action GameStarts;

    private int activeScreenRezIndex;
    
    private void Start()
    {
        activeScreenRezIndex = PlayerPrefs.GetInt("screen res index");
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen")==1;

        sliders[0].value = AudioManager.instance.masterVolumePercent;
        sliders[1].value = AudioManager.instance.musicVolumePercent;
        sliders[2].value = AudioManager.instance.sfxVolumePercent;

        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = i == activeScreenRezIndex;
        }

        fullsceenToggle.isOn = isFullscreen;
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
        GameStarts?.Invoke();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OptionsMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void MainMenu()
    {
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }

    public void SetScreenResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            activeScreenRezIndex = i;
            float aspectRation = 16 / 9f;
            Screen.SetResolution(screenWidths[i],(int)(screenWidths[i]/aspectRation), false);
            PlayerPrefs.SetInt("screen res index", activeScreenRezIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        for(int i=0; i<resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullscreen;
        }

        if (isFullscreen)
        {

            Resolution[] allResolutionses = Screen.resolutions;
            Resolution maxResolution = allResolutionses[allResolutionses.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            SetScreenResolution(activeScreenRezIndex);
        }
        
        PlayerPrefs.SetInt("fullscreen", isFullscreen?1:0);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }
}
