using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelManager : MonoBehaviour
{
    public Button toggleButton; // Button to toggle the audio
    public Sprite onSprite; // Sprite to indicate "On" state
    public Sprite offSprite; // Sprite to indicate "Off" state
    public Slider volumeSlider; // Slider to control the volume

    private AudioSource persistedAudioSource; // Reference to the AudioSource from the persisted GameObject

    private void Start()
    {
        // Find the persisted Audio GameObject from the Homepage scene
        GameObject audioManager = GameObject.Find("BG Music"); // Replace "Audio" with the exact name of your persisted GameObject

        if (audioManager != null)
        {
            // Access the AudioSource on the persisted Audio GameObject
            persistedAudioSource = audioManager.GetComponent<AudioSource>();

            if (persistedAudioSource != null)
            {
                // Initialize the audio source volume to 80% if not already set
                float initialVolume = 0.8f;
                persistedAudioSource.volume = initialVolume;

                // Initialize the button state
                UpdateButtonSprite();
                toggleButton.onClick.AddListener(ToggleAudio);

                // Initialize the volume slider
                if (volumeSlider != null)
                {
                    volumeSlider.value = initialVolume; // Set slider to 80% volume initially
                    UpdateSliderFill(volumeSlider); // Ensure the slider's fill level matches
                    volumeSlider.onValueChanged.AddListener(SetVolume);
                }
            }
            else
            {
                Debug.LogError("AudioSource component not found on the Audio GameObject!");
            }
        }
        else
        {
            Debug.LogError("Persisted Audio GameObject not found!");
        }
    }

    private void ToggleAudio()
    {
        if (persistedAudioSource != null)
        {
            // Toggle the mute state of the audio source
            persistedAudioSource.mute = !persistedAudioSource.mute;

            // Update the button sprite to reflect the new state
            UpdateButtonSprite();
        }
    }

    private void UpdateButtonSprite()
    {
        if (persistedAudioSource != null)
        {
            // Set the button's image to match the mute state
            Image buttonImage = toggleButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = persistedAudioSource.mute ? offSprite : onSprite;
            }
        }
    }

    private void SetVolume(float volume)
    {
        if (persistedAudioSource != null)
        {
            // Adjust the volume of the persisted audio source
            persistedAudioSource.volume = volume;

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
