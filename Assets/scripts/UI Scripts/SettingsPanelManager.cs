using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelManager : MonoBehaviour
{
    public Button toggleButton; // Button to toggle the audio
    public Sprite onSprite; // Sprite to indicate "On" state
    public Sprite offSprite; // Sprite to indicate "Off" state
    public Slider volumeSlider; // Slider to control the volume

    private AudioSource persistedAudioSource; // Reference to the AudioSource from the persisted GameObject
    private AudioSource musicAudioSource;

    private void Start()
    {
        GameObject audioManager = GameObject.Find("BG Music"); // Replace "BG Music" with the exact name of your persisted GameObject

        if (audioManager != null)
        {
            // Access the AudioSource on the persisted Audio GameObject
            persistedAudioSource = audioManager.GetComponent<AudioSource>();

            if (persistedAudioSource != null)
            {
                // Initialize the audio source volume to 80% if not already set
                float initialVolume = 0.8f;
                persistedAudioSource.volume = initialVolume;

                // Ensure the fallback Music GameObject is inactive
                GameObject musicGameObject = GameObject.Find("Music");
                if (musicGameObject != null)
                {
                    musicGameObject.SetActive(false);
                }

                // Initialize the button state
                UpdateButtonSprite(persistedAudioSource);
                toggleButton.onClick.AddListener(() => ToggleAudio(persistedAudioSource));

                // Initialize the volume slider
                if (volumeSlider != null)
                {
                    volumeSlider.value = initialVolume; // Set slider to 80% volume initially
                    UpdateSliderFill(volumeSlider); // Ensure the slider's fill level matches
                    volumeSlider.onValueChanged.AddListener((value) => SetVolume(value, persistedAudioSource));
                }
            }
            else
            {
                Debug.LogError("AudioSource component not found on the BG Music GameObject!");
            }
        }
        else
        {
            Debug.LogWarning("Persisted Audio GameObject not found. Attempting to play audio from Music GameObject.");

            // Find the "Music" GameObject as a fallback
            GameObject musicGameObject = GameObject.Find("Music"); // Replace "Music" with the actual name of the fallback GameObject

            if (musicGameObject != null)
            {
                musicAudioSource = musicGameObject.GetComponent<AudioSource>();

                if (musicAudioSource != null)
                {
                    // Ensure the persisted BG Music GameObject is inactive
                    GameObject audioManagerInactive = GameObject.Find("BG Music");
                    if (audioManagerInactive != null)
                    {
                        audioManagerInactive.SetActive(false);
                    }

                    float fallbackVolume = 0.8f; // Set default volume for fallback audio
                    musicAudioSource.volume = fallbackVolume;

                    // Play the audio
                    if (!musicAudioSource.isPlaying)
                    {
                        musicAudioSource.Play();
                    }

                    // Optionally, initialize slider and button state for the fallback AudioSource
                    if (volumeSlider != null)
                    {
                        volumeSlider.value = fallbackVolume;
                        UpdateSliderFill(volumeSlider);
                        volumeSlider.onValueChanged.AddListener((value) => SetVolume(value, musicAudioSource));
                    }

                    toggleButton.onClick.AddListener(() => ToggleAudio(musicAudioSource));

                    // Ensure the button sprite updates correctly
                    UpdateButtonSprite(musicAudioSource);
                }
                else
                {
                    Debug.LogError("AudioSource component not found on the Music GameObject!");
                }
            }
            else
            {
                Debug.LogError("Music GameObject not found!");
            }
        }
    }

    private void ToggleAudio(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            // Toggle the mute state of the audio source
            audioSource.mute = !audioSource.mute;

            // Update the button sprite to reflect the new state
            UpdateButtonSprite(audioSource);
        }
    }

    private void UpdateButtonSprite(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            // Set the button's image to match the mute state
            Image buttonImage = toggleButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = audioSource.mute ? offSprite : onSprite;
            }
        }
    }

    private void SetVolume(float volume, AudioSource audioSource)
    {
        if (audioSource != null)
        {
            // Adjust the volume of the audio source
            audioSource.volume = volume;

            // Update the slider's fill level (useful if a custom fill image is used)
            UpdateSliderFill(volumeSlider);
        }
    }

    private void UpdateSliderFill(Slider slider)
    {
        // Update the visual fill level of the slider
        if (slider.fillRect != null)
        {
            slider.fillRect.GetComponent<Image>().fillAmount = slider.value;
        }
    }
}
