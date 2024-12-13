using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // For scene loading

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public Button levelButton; // The button for the level
        public GameObject levelCard; // The corresponding level card popup
    }

    public LevelData[] levels; // Array to hold all levels
    public Button homepageButton; // Homepage button
    public Button settingsButton; // Settings button
    public GameObject settingsPanel; // Settings popup panel
    public float animationDuration = 0.5f; // Duration for animations
    public Image backgroundImage; // Reference to the background image

    private void Start()
    {
        // Initialize level buttons to open respective popups
        foreach (var level in levels)
        {
            level.levelCard.SetActive(false); // Ensure all level cards are hidden initially
            level.levelButton.onClick.AddListener(() => OpenLevelCard(level));
        }

        // Initialize Homepage and Settings buttons
        if (homepageButton != null)
        {
            homepageButton.onClick.AddListener(OpenHomepage);
        }
        if (settingsButton != null && settingsPanel != null)
        {
            settingsPanel.SetActive(false); // Ensure the settings panel is hidden initially
            settingsButton.onClick.AddListener(OpenSettingsPanel);
        }
    }

    private void OpenLevelCard(LevelData selectedLevel)
    {
        // Dim the background when a level card is opened
        DimBackground(true);

        // Animate the level button to scale larger
        AnimateButtonScale(selectedLevel.levelButton);

        // Animate the level card opening
        AnimateOpen(selectedLevel.levelCard);

        // Disable all buttons except the active level's button
        foreach (var level in levels)
        {
            level.levelButton.interactable = (level == selectedLevel);
        }

        // Disable Homepage and Settings buttons
        SetGlobalButtonsState(false);

        // Add close button logic for the active level card
        var closeButton = selectedLevel.levelCard.GetComponentInChildren<Button>();
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners(); // Avoid stacking listeners
            closeButton.onClick.AddListener(() => CloseLevelCard(selectedLevel, closeButton));
        }
        else
        {
            Debug.LogError("Close button not found on level card!");
        }
    }

    private void CloseLevelCard(LevelData selectedLevel, Button closeButton)
    {
        // Dim the background back to normal
        DimBackground(false);

        // Animate the level button to scale down with easeOut
        AnimateButtonScaleDown(selectedLevel.levelButton);

        // Animate the close button scaling down (optional for more effect)
        AnimateButtonScaleDown(closeButton);

        // Animate the level card closing
        AnimateClose(selectedLevel.levelCard, () =>
        {
            // Enable all level buttons
            foreach (var level in levels)
            {
                level.levelButton.interactable = true;
            }

            // Re-enable Homepage and Settings buttons
            SetGlobalButtonsState(true);
        });
    }

    private void AnimateButtonScale(Button button)
    {
        // Animate the button to scale larger
        RectTransform rect = button.GetComponent<RectTransform>();
        if (rect != null)
        {
            LeanTween.scale(rect, Vector3.one * 1.2f, animationDuration / .5f) // Scale up
                     .setEase(LeanTweenType.easeOutBack);
        }
    }

    private void AnimateButtonScaleDown(Button button)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        if (rect != null)
        {
            LeanTween.scale(rect, Vector3.one, animationDuration / 1.5f) // Scale down to normal size
                     .setEase(LeanTweenType.easeOutBack);  // Use easeOut for smooth animation
        }
    }

    private void OpenSettingsPanel()
    {
        // Dim the background when settings panel is opened
        DimBackground(true);

        // Animate the settings panel opening
        AnimateOpen(settingsPanel);

        // Disable all level buttons
        foreach (var level in levels)
        {
            level.levelButton.interactable = false;
        }

        // Disable Homepage button
        SetGlobalButtonsState(false);

        // Add close button logic for the settings panel
        var closeButton = settingsPanel.GetComponentInChildren<Button>();
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners(); // Avoid stacking listeners
            closeButton.onClick.AddListener(CloseSettingsPanel);
        }
        else
        {
            Debug.LogError("Close button not found on settings panel!");
        }
    }

    private void CloseSettingsPanel()
    {
        // Dim the background back to normal
        DimBackground(false);

        // Animate the settings panel closing
        AnimateClose(settingsPanel, () =>
        {
            // Re-enable all level buttons
            foreach (var level in levels)
            {
                level.levelButton.interactable = true;
            }

            // Re-enable Homepage button
            SetGlobalButtonsState(true);
        });
    }

    private void SetGlobalButtonsState(bool state)
    {
        if (homepageButton != null) homepageButton.interactable = state;
        if (settingsButton != null) settingsButton.interactable = state;
    }

    private void OpenHomepage()
    {
        Debug.Log("Navigating to the Homepage (Scene Index 0).");
        SceneManager.LoadScene(0); // Change to the scene at index 0
    }

    // Animation for opening panels/cards
    private void AnimateOpen(GameObject panel)
    {
        panel.SetActive(true);
        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = Vector3.zero; // Start from a scale of 0
            LeanTween.scale(rect, Vector3.one, animationDuration).setEase(LeanTweenType.easeOutBack);
        }
    }

    // Animation for closing panels/cards
    private void AnimateClose(GameObject panel, System.Action onComplete)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect != null)
        {
            LeanTween.scale(rect, Vector3.zero, animationDuration)
                .setEase(LeanTweenType.easeInBack)
                .setOnComplete(() =>
                {
                    panel.SetActive(false); // Hide the panel after animation
                    onComplete?.Invoke(); // Execute the callback
                });
        }
        else
        {
            panel.SetActive(false); // Fallback if no RectTransform is found
            onComplete?.Invoke();
        }
    }

    // Method to dim or restore the background image
    private void DimBackground(bool dim)
    {
        if (backgroundImage != null)
        {
            Color color = backgroundImage.color;
            color.a = dim ? 0.5f : 1f; // Dim to 50% opacity or restore to full opacity
            backgroundImage.color = color;
        }
    }
}
