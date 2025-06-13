using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelManager : MonoBehaviour
{
    public Button toggleButton;
    public Sprite onSprite;
    public Sprite offSprite;
    public Slider volumeSlider;

    private AudioSource persistedAudioSource;
    private AudioSource musicAudioSource;

    private const string VolumeKey = "AudioVolume";
    private const string MuteKey = "AudioMute";

    private void Start()
    {
        GameObject audioManager = GameObject.Find("BG Music");

        if (audioManager != null)
        {
            persistedAudioSource = audioManager.GetComponent<AudioSource>();

            if (persistedAudioSource != null)
            {
                float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.8f);
                bool isMuted = PlayerPrefs.GetInt(MuteKey, 0) == 1;

                persistedAudioSource.volume = savedVolume;
                persistedAudioSource.mute = isMuted;

                GameObject musicGameObject = GameObject.Find("Music");
                if (musicGameObject != null) musicGameObject.SetActive(false);

                UpdateButtonSprite(persistedAudioSource);
                toggleButton.onClick.AddListener(() => ToggleAudio(persistedAudioSource));

                if (volumeSlider != null)
                {
                    volumeSlider.value = savedVolume;
                    UpdateSliderFill(volumeSlider);
                    volumeSlider.onValueChanged.AddListener((value) => SetVolume(value, persistedAudioSource));
                }
            }
        }
        else
        {
            Debug.LogWarning("Persisted Audio GameObject not found. Attempting fallback.");
            GameObject musicGameObject = GameObject.Find("Music");

            if (musicGameObject != null)
            {
                musicAudioSource = musicGameObject.GetComponent<AudioSource>();

                if (musicAudioSource != null)
                {
                    GameObject audioManagerInactive = GameObject.Find("BG Music");
                    if (audioManagerInactive != null) audioManagerInactive.SetActive(false);

                    float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.8f);
                    bool isMuted = PlayerPrefs.GetInt(MuteKey, 0) == 1;

                    musicAudioSource.volume = savedVolume;
                    musicAudioSource.mute = isMuted;

                    if (!musicAudioSource.isPlaying)
                        musicAudioSource.Play();

                    if (volumeSlider != null)
                    {
                        volumeSlider.value = savedVolume;
                        UpdateSliderFill(volumeSlider);
                        volumeSlider.onValueChanged.AddListener((value) => SetVolume(value, musicAudioSource));
                    }

                    toggleButton.onClick.AddListener(() => ToggleAudio(musicAudioSource));
                    UpdateButtonSprite(musicAudioSource);
                }
            }
        }
    }

    private void ToggleAudio(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.mute = !audioSource.mute;
            PlayerPrefs.SetInt(MuteKey, audioSource.mute ? 1 : 0);
            PlayerPrefs.Save();

            UpdateButtonSprite(audioSource);
        }
    }

    private void UpdateButtonSprite(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            Image buttonImage = toggleButton.GetComponent<Image>();
            if (buttonImage != null)
                buttonImage.sprite = audioSource.mute ? offSprite : onSprite;
        }
    }

    private void SetVolume(float volume, AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
            PlayerPrefs.SetFloat(VolumeKey, volume);
            PlayerPrefs.Save();

            UpdateSliderFill(volumeSlider);
        }
    }

    private void UpdateSliderFill(Slider slider)
    {
        if (slider.fillRect != null)
            slider.fillRect.GetComponent<Image>().fillAmount = slider.value;
    }
}
