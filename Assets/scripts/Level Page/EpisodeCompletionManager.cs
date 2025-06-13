using UnityEngine;
using UnityEngine.UI;

public class EpisodeCompletionManager : MonoBehaviour
{
    [System.Serializable]
    public class EpisodeData
    {
        public string episodeKey;  // Key to track completion (e.g., "EP1_Completed")
        public Button episodeButton; // Button to enable when the episode is completed
        public Image lockImage; // Lock icon to show when the episode is locked
        public Sprite lockedSprite; // Sprite for locked state
        public Sprite unlockedSprite; // Sprite for unlocked state
    }

    public EpisodeData[] episodes; // Array to store all episodes

    void Start()
    {
        foreach (var episode in episodes)
        {
            bool isCompleted = PlayerPrefs.GetInt(episode.episodeKey, 0) == 1;

            // Ensure the button is always visible but not interactable when locked
            episode.episodeButton.gameObject.SetActive(true);
            episode.episodeButton.interactable = isCompleted;

            // Update the lock image sprite
            if (episode.lockImage != null)
            {
                episode.lockImage.sprite = isCompleted ? episode.unlockedSprite : episode.lockedSprite;
            }
        }
    }

    // Call this function when an episode is completed to unlock its corresponding button
    public void MarkEpisodeAsCompleted(string episodeKey)
    {
        PlayerPrefs.SetInt(episodeKey, 1); // Save progress
        PlayerPrefs.Save(); // Ensure changes are written

        // Update button state dynamically
        foreach (var episode in episodes)
        {
            if (episode.episodeKey == episodeKey)
            {
                episode.episodeButton.interactable = true;
                if (episode.lockImage != null)
                {
                    episode.lockImage.sprite = episode.unlockedSprite;
                }
                break;
            }
        }
    }
}
