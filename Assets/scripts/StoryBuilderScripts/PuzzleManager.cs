using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public GameObject[] puzzlePieces; // Array of puzzle pieces
    public GameObject[] dropSlots; // Array of drop slots
    public GameObject[] incorrectIndicators; // Sprites that indicate incorrect placement
    public GameObject congratulationsPanel; // UI panel for "Congratulations"

    private Vector3[] originalPositions; // Stores original positions of puzzle pieces
    private Vector2[] originalSizeDeltas; // Stores original sizeDeltas
    private Quaternion[] originalRotation; // Stores original rotation
    public Transform originalParent; // Reference to the original parent for all pieces

    public bool isShowingCongratulations = false; // Prevents multiple animations
    private int mistakesMade = 0; // Tracks mistakes for star rating
    int maxMistakesMade = 0;
    private string levelKey; // Unique key for PlayerPrefs storage


    public AudioSource bgmAudioSource;              // Background music
    public AudioSource sfxAudioSource;              // For sound effects
    public AudioClip congratulationsSFX;            // Assign in Inspector

    public GameObject particleEffectPrefab1; // e.g. confetti


    public Transform particleSpawnPoint1; // Optional: custom spawn location

    public AudioClip mistakeSound;

    private bool puzzleChecked = false;
    private float lastMistakeSoundTime = 0f;
    private float mistakeSoundCooldown = 0.5f;
    bool hasPlayedMistakeSound = false; // Flag to track if mistake sound has been played for the current check






    void Start()
    {
        originalPositions = new Vector3[puzzlePieces.Length];
        originalSizeDeltas = new Vector2[puzzlePieces.Length];
        originalRotation = new Quaternion[puzzlePieces.Length];
        congratulationsPanel.SetActive(false);

        levelKey = "Level" + SceneManager.GetActiveScene().buildIndex + "_Stars"; // Unique key per level
        Debug.Log("Level Key: " + levelKey); // Debugging Level Key

        // Store original positions, sizes, and rotations
        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            RectTransform rectTransform = puzzlePieces[i].GetComponent<RectTransform>();
            originalPositions[i] = rectTransform.position;
            originalSizeDeltas[i] = rectTransform.sizeDelta;
            originalRotation[i] = rectTransform.rotation;
        }


    }

    void Update()
    {
        if (!isShowingCongratulations && AllDropSlotsFilled())
        {
            CheckPuzzleCompletion();
            
            Debug.Log("Updating");
        }

    }

    private bool AllDropSlotsFilled()
    {
        foreach (GameObject slot in dropSlots)
        {
            if (slot.transform.childCount == 1) // Slot is empty
            {
                return false;
            }
        }
        return true; // All slots filled
    }


    void CheckPuzzleCompletion()
    {
        bool allCorrect = true; // Assume all correct initially
        mistakesMade = 0; // Reset mistake counter

        

        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            GameObject currentPiece = puzzlePieces[i];

            if (!IsPieceInCorrectSlot(currentPiece, i))
            {
                mistakesMade++; // Count incorrect placement
                Debug.Log("Incorrect Placement: " + currentPiece.name); // Debugging

                SetIncorrectIndicator(incorrectIndicators[i], currentPiece);

                // Play fail sound once per check
                if (mistakeSound != null && sfxAudioSource != null && !hasPlayedMistakeSound)
                {
                    sfxAudioSource.PlayOneShot(mistakeSound); // Play fail sound
                    hasPlayedMistakeSound = true; // Set the flag to true to prevent multiple plays
                }

                StartCoroutine(ResetPieceAfterDelay(currentPiece, i, 3)); // Reset after 3s

                allCorrect = false;
            }
            else
            {
                incorrectIndicators[i].SetActive(false); // Hide incorrect indicator
            }
        }

        // Update max mistakes made
        if (mistakesMade > maxMistakesMade)
        {
            maxMistakesMade = mistakesMade;
        }

        Debug.Log("Total Mistakes: " + mistakesMade); // Debugging
        Debug.Log("Max Mistakes Made: " + maxMistakesMade); // Debugging

        if (allCorrect)
        {
            ShowCongratulationsMessage();
            CalculateStars();
        }
    }



    void ShowCongratulationsMessage()
    {
        if (!isShowingCongratulations)
        {
            isShowingCongratulations = true;

            // Reset scale & activate panel
            congratulationsPanel.transform.localScale = Vector3.zero;
            congratulationsPanel.SetActive(true);

            // Play congratulations sound
            sfxAudioSource.PlayOneShot(congratulationsSFX);

            // Animate popup
            LeanTween.scale(congratulationsPanel, Vector3.one, 0.5f)
                     .setEase(LeanTweenType.easeOutBack);

            // Fade down BGM volume
            LeanTween.value(bgmAudioSource.gameObject, bgmAudioSource.volume, 0.2f, 3f)
                     .setOnUpdate((float val) => {
                         bgmAudioSource.volume = val;
                     });

            // Spawn particle effect 1
            Vector3 spawnPos1 = particleSpawnPoint1 != null
                ? particleSpawnPoint1.position
                : congratulationsPanel.transform.position;

            Instantiate(particleEffectPrefab1, spawnPos1, Quaternion.identity);

            // Disable DragHandlerControl on GameObjects "1" to "9"
            for (int i = 1; i <= 9; i++)
            {
                GameObject obj = GameObject.Find(i.ToString());
                if (obj != null)
                {
                    DragHandlerControl handler = obj.GetComponent<DragHandlerControl>();
                    if (handler != null)
                    {
                        handler.enabled = false;
                    }
                }
            }
        }
    }


    public void RestoreBGMVolume()
    {
        LeanTween.value(bgmAudioSource.gameObject, bgmAudioSource.volume, 1.0f, 2f)
                 .setOnUpdate((float val) => {
                     bgmAudioSource.volume = val;
                 });
    }


    void CalculateStars()
    {
        Debug.Log("Max Mistakes Made: " + maxMistakesMade);
        int stars = 3; // Default to 3 stars

        if (maxMistakesMade > 3)
            stars = 1; // More than 3 mistakes = 1 star
        else if (maxMistakesMade > 1)
            stars = 2; // 2-3 mistakes = 2 stars

        int savedStars = PlayerPrefs.GetInt(levelKey, 0);
        if (stars > savedStars)
        {
            PlayerPrefs.SetInt(levelKey, stars); // Save highest stars
            PlayerPrefs.Save();
        }

        Debug.Log("Stars Saved: " + stars + " for " + levelKey);
    }

    IEnumerator ResetPieceAfterDelay(GameObject piece, int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        RectTransform rectTransform = piece.GetComponent<RectTransform>();
        rectTransform.SetParent(originalParent, false);
        rectTransform.position = originalPositions[index];
        rectTransform.sizeDelta = originalSizeDeltas[index];
        rectTransform.localRotation = originalRotation[index];
        incorrectIndicators[index].SetActive(false); // Hide incorrect indicator
        Debug.Log("Resetting Piece: " + piece.name); // Debugging
        hasPlayedMistakeSound = false; // Allow sound again after piece resets

    }

    void SetIncorrectIndicator(GameObject indicator, GameObject puzzlePiece)
    {
        RectTransform puzzleRect = puzzlePiece.GetComponent<RectTransform>();
        RectTransform indicatorRect = indicator.GetComponent<RectTransform>();

        indicatorRect.position = puzzleRect.position;
        indicatorRect.sizeDelta = puzzleRect.sizeDelta;
        indicator.SetActive(true);
    }

    bool IsPieceInCorrectSlot(GameObject piece, int index)
    {
        bool isCorrect = piece.transform.parent.name == (index + 1).ToString() + "S";
        Debug.Log("Checking Piece: " + piece.name + " | Parent: " + piece.transform.parent.name + " | Correct: " + isCorrect);
        return isCorrect;
    }
}