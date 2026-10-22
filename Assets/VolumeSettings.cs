using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (AudioManager.Instance == null) return;

        // Set the value first, then listen, so it doesn't trigger a change
        musicSlider.value = AudioManager.Instance.musicVolume;
        sfxSlider.value = AudioManager.Instance.sfxVolume;

        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSfxVolume);
    }
}