using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManu : MonoBehaviour
{
    public AudioMixer AudioMixer;
    Resolution[] resolutions;
    public TMP_Dropdown ResolutionDropdown;
    
    
    private void Start()
    {
        int CurrentResolutionIndex = 0;
        resolutions = Screen.resolutions;
        
        ResolutionDropdown.ClearOptions();
        
        List<string> options = new List<string>();
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            string Option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(Option);
            
            if(resolutions[i].width == Screen.currentResolution.width &&
               resolutions[i].height == Screen.currentResolution.height)
            {
                CurrentResolutionIndex = i;
            }
        }
        
        ResolutionDropdown.AddOptions(options);
        ResolutionDropdown.value = CurrentResolutionIndex;
        ResolutionDropdown.RefreshShownValue();
    }
    
    public void SetResolution(int ResolutionIndex)
    {
        Resolution resolution = resolutions[ResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    
    public void SetVolume(float volume)
    {
        AudioMixer.SetFloat("volume", volume);
    }
    
    
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    public void SetFullscreen(bool isFullscreen)
    {
        // NOTE(sftl): Unity full screen is buggy. It remembers the size of the window and sceles to fit full screen...
        // Maybe we should enable playing in full screen only. There is a fix:
        // https://gamesfromearth.medium.com/dealing-with-fullscreen-in-unity-43b7673f9732
        Screen.fullScreen = isFullscreen;
    }
}