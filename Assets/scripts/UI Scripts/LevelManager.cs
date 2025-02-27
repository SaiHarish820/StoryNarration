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
        public Button nextButton; // Button to navigate to the next level
        public Button previousButton; // Button to navigate to the previous level
        public Button gameButton; // Button to start the game
        public string sceneName; // Scene to load when this game button is pressed
        public Button storyButton; // Button to start the story
        public string episodeName; // Scene to load when this episode button is pressed
    }

    public LevelData[] levels; // Array to hold all levels
    public Button homepageButton; // Homepage button
    public Button settingsButton; // Settings button
    public GameObject settingsPanel; // Settings popup panel
    public float animationDuration = 0.5f; // Duration for animations
    public Image backgroundImage; // Reference to the background image
    public GameObject closeArea; // Transparent area for closing level cards
    public GameObject informationPanel; // Information panel reference
    public Button informationButton; // Information button reference

    public AudioSource sfxAudioSource; // Audio source for playing SFX
    public AudioClip buttonClickSFX; // SFX for button clicks
    public AudioClip navigateSFX; // SFX for navigating levels
    public AudioClip closePanelSFX; // SFX for closing panels

    private int currentLevelIndex = -1; // Tracks the currently active level

    private static bool isAudioSourceInitialized = false; // To prevent duplicate audio sources

    private void Awake()
    {
        // Ensure the AudioSource persists across scenes
        if (!isAudioSourceInitialized && sfxAudioSource != null)
        {
            DontDestroyOnLoad(sfxAudioSource.gameObject);
            isAudioSourceInitialized = true;
        }
    }

    private void Start()
    {
        InitializeLevelButtons();
        InitializeGlobalButtons();
        InitializeCloseArea();
        InitializeInformationButton(); // Initialize the button
    }

    private void InitializeInformationButton()
    {
        informationPanel.SetActive(false);
        if (informationButton != null && informationPanel != null)
        {
            informationButton.onClick.AddListener(() =>
            {
                PlaySFX(buttonClickSFX);
                OpenInformationPanel();
            });
        }
    }

    private void OpenInformationPanel()
    {
        DimBackground(true);
        AnimateOpen(informationPanel);

        if (closeArea != null)
        {
            closeArea.SetActive(true);
            closeArea.GetComponent<Button>().onClick.AddListener(CloseInformationPanel);
        }

        foreach (var level in levels)
        {
            level.levelButton.interactable = false;
        }

        SetGlobalButtonsState(false);
    }

    private void CloseInformationPanel()
    {
        DimBackground(false);
        AnimateClose(informationPanel, () =>
        {
            informationPanel.SetActive(false);
            if (closeArea != null)
            {
                closeArea.SetActive(false);
                closeArea.GetComponent<Button>().onClick.RemoveListener(CloseInformationPanel);
            }

            foreach (var level in levels)
            {
                level.levelButton.interactable = true;
            }

            SetGlobalButtonsState(true);
        });
    }

    private void InitializeLevelButtons()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            var levelIndex = i; // Capture index for lambda expression
            levels[i].levelCard.SetActive(false); // Ensure all level cards are hidden initially
            levels[i].levelButton.onClick.AddListener(() =>
            {
                PlaySFX(buttonClickSFX);
                OpenLevelCard(levelIndex);
            });

            if (levels[i].nextButton != null)
            {
                levels[i].nextButton.onClick.AddListener(() =>
                {
                    PlaySFX(navigateSFX);
                    NavigateToNextLevel(levelIndex);
                });
            }

            if (levels[i].previousButton != null)
            {
                levels[i].previousButton.onClick.AddListener(() =>
                {
                    PlaySFX(navigateSFX);
                    NavigateToPreviousLevel(levelIndex);
                });
            }

            // Configure the game button to load the game scene
            if (levels[i].gameButton != null)
            {
                levels[i].gameButton.onClick.AddListener(() =>
                {
                    PlaySFX(buttonClickSFX);
                    LoadScene(levels[levelIndex].sceneName);
                });
            }

            // Configure the story button to load the episode scene
            if (levels[i].storyButton != null)
            {
                levels[i].storyButton.onClick.AddListener(() =>
                {
                    PlaySFX(buttonClickSFX);
                    LoadScene(levels[levelIndex].episodeName);
                });
            }
        }
    }


    private void InitializeGlobalButtons()
    {
        if (homepageButton != null)
        {
            homepageButton.onClick.AddListener(() =>
            {
                PlaySFX(buttonClickSFX);
                OpenHomepage();
            });
        }

        if (settingsButton != null && settingsPanel != null)
        {
            settingsPanel.SetActive(false); // Ensure the settings panel is hidden initially
            settingsButton.onClick.AddListener(() =>
            {
                PlaySFX(buttonClickSFX);
                OpenSettingsPanel();
            });
        }

        if (informationButton!= null && informationPanel != null)
        {
            informationPanel.SetActive(false); // Ensure the information panel is hidden initially
            informationButton.onClick.AddListener(() =>
            {
                PlaySFX(buttonClickSFX);
                OpenInformationPanel();
            });
        }
    }

    private void InitializeCloseArea()
    {
        if (closeArea != null)
        {
            closeArea.SetActive(false); // Hidden initially
            closeArea.GetComponent<Button>().onClick.AddListener(() =>
            {
                PlaySFX(closePanelSFX);
                CloseLevelCard();
            });
        }
    }

    private void OpenLevelCard(int levelIndex)
    {
        if (currentLevelIndex != -1)
        {
            levels[currentLevelIndex].levelCard.SetActive(false);
        }

        currentLevelIndex = levelIndex;
        DimBackground(true);
        AnimateOpen(levels[levelIndex].levelCard);

        if (closeArea != null) closeArea.SetActive(true);
        SetGlobalButtonsState(false);
    }

    private void NavigateToNextLevel(int currentLevelIndex)
    {
        int nextLevelIndex = (currentLevelIndex + 1) % levels.Length;
        OpenLevelCard(nextLevelIndex);
    }

    private void NavigateToPreviousLevel(int currentLevelIndex)
    {
        int previousLevelIndex = (currentLevelIndex - 1 + levels.Length) % levels.Length;
        OpenLevelCard(previousLevelIndex);
    }

    public void CloseLevelCard()
    {
        if (currentLevelIndex != -1)
        {
            DimBackground(false);
            AnimateClose(levels[currentLevelIndex].levelCard, null);

            currentLevelIndex = -1;
            if (closeArea != null) closeArea.SetActive(false);
            SetGlobalButtonsState(true);
        }
    }

    private void AnimateOpen(GameObject panel)
    {
        panel.SetActive(true);
        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = Vector3.zero;
            LeanTween.scale(rect, Vector3.one, animationDuration).setEase(LeanTweenType.easeOutBack);
        }
    }

    private void AnimateClose(GameObject panel, System.Action onComplete)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect != null)
        {
            LeanTween.scale(rect, Vector3.zero, animationDuration)
                .setEase(LeanTweenType.easeInBack)
                .setOnComplete(() =>
                {
                    panel.SetActive(false);
                    onComplete?.Invoke();
                });
        }
        else
        {
            panel.SetActive(false);
            onComplete?.Invoke();
        }
    }

    private void DimBackground(bool dim)
    {
        if (backgroundImage != null)
        {
            Color color = backgroundImage.color;
            color.a = dim ? 0.5f : 1f;
            backgroundImage.color = color;
        }
    }

    private void OpenSettingsPanel()
    {
        DimBackground(true);
        AnimateOpen(settingsPanel);

        if (closeArea != null)
        {
            closeArea.SetActive(true);
            closeArea.GetComponent<Button>().onClick.AddListener(CloseSettingsPanel);
        }

        foreach (var level in levels)
        {
            level.levelButton.interactable = false;
        }

        SetGlobalButtonsState(false);
    }

    private void CloseSettingsPanel()
    {
        DimBackground(false);
        AnimateClose(settingsPanel, () =>
        {
            settingsPanel.SetActive(false);
            if (closeArea != null)
            {
                closeArea.SetActive(false);
                closeArea.GetComponent<Button>().onClick.RemoveListener(CloseSettingsPanel);
            }

            foreach (var level in levels)
            {
                level.levelButton.interactable = true;
            }

            SetGlobalButtonsState(true);
        });
    }

    private void SetGlobalButtonsState(bool state)
    {
        if (homepageButton != null) homepageButton.interactable = state;
        if (settingsButton != null) settingsButton.interactable = state;
        if(informationButton != null) informationButton.interactable = state;
    }

    private void OpenHomepage()
    {
        PlaySFX(buttonClickSFX);
        SceneManager.LoadScene(0);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxAudioSource != null && clip != null)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }

    private void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is not set for the game button.");
        }
    }

}
