using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PuzzleManager : MonoBehaviour
{
    public GameObject[] puzzlePieces; // Array to store the puzzle pieces
    public GameObject[] dropSlots; // Array to store the drop slots
    public GameObject[] incorrectIndicators; // Array to store the sprites that indicate an incorrect placement
    public Vector3[] originalPositions; // Array to store original positions of each puzzle piece
    public Vector2[] originalSizeDeltas; // Array to store original sizeDeltas (added for maintaining size)
    public Quaternion[] originalRotation; // Array to store original Rotation (added for maintaining rotation)
    public Transform originalParent; // Reference to the original parent for all pieces
    public GameObject congratulationsPanel; // Reference to the GameObject with the Text UI "Congratulations"

    public bool isShowingCongratulations = false; // Flag to prevent multiple animations
    


    void Start()
    {
        originalPositions = new Vector3[puzzlePieces.Length];
        originalSizeDeltas = new Vector2[puzzlePieces.Length];
        originalRotation = new Quaternion[puzzlePieces.Length]; // Initialize the rotation array
        congratulationsPanel.SetActive(false);

        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            RectTransform rectTransform = puzzlePieces[i].GetComponent<RectTransform>();
            originalPositions[i] = rectTransform.position;
            originalSizeDeltas[i] = rectTransform.sizeDelta; // Store the original size
            originalRotation[i] = rectTransform.rotation; // Store the original rotation
        }
    }

    void Update()
    {
        if (isShowingCongratulations == false && AllDropSlotsFilled())
        {
            CheckPuzzleCompletion();
            Debug.Log("Checking Puzzle");
        }
    }


    private bool AllDropSlotsFilled()
    {
        foreach (GameObject slot in dropSlots)
        {
            if (slot.transform.childCount == 1)
            {
                return false; // At least one slot is empty
            }
        }
        return true; // All slots are filled
    }

    void CheckPuzzleCompletion()
    {
        bool allCorrect = true;
        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            GameObject currentPiece = puzzlePieces[i];
            if (!IsPieceInCorrectSlot(currentPiece, i))
            {
                SetIncorrectIndicator(incorrectIndicators[i], currentPiece); // Position and show incorrect indicator
                StartCoroutine(ResetPieceAfterDelay(currentPiece, i, 3)); // 3 seconds delay
                allCorrect = false;
            }
            else
            {
                incorrectIndicators[i].SetActive(false); // Hide incorrect status sprite if correctly placed
            }
        }

        if (allCorrect)
        {
            Debug.Log("Congratulations! Puzzle completed successfully.");
            ShowCongratulationsMessage();
        }
    }

    void ShowCongratulationsMessage()
    {
        if (!isShowingCongratulations)
        {
            isShowingCongratulations = true;
            congratulationsPanel.SetActive(true);
           /* LeanTween.cancel(congratulationsPanel); // Cancel any existing animations
            LeanTween.scale(congratulationsPanel, new Vector3(1, 1, 1), 0.5f) // Scale down to normal size
                .setFrom(new Vector3(1.5f, 1.5f, 1.5f)) // Start from a larger size
                .setEase(LeanTweenType.easeOutBack) // Use an ease out to make the zoom out smooth
                .setOnComplete(() => {
                    isShowingCongratulations = false; // Reset the flag when animation completes
                });*/
        }
        Debug.Log(isShowingCongratulations);
    }



    IEnumerator ResetPieceAfterDelay(GameObject piece, int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        RectTransform rectTransform = piece.GetComponent<RectTransform>();
        rectTransform.SetParent(originalParent, false);
        rectTransform.position = originalPositions[index];
        rectTransform.sizeDelta = originalSizeDeltas[index]; // Reset to original size
        rectTransform.localRotation = originalRotation[index]; // Reset to original rotation
        incorrectIndicators[index].SetActive(false); // Hide the incorrect sprite once reset
        Debug.Log("Resetting piece " + piece.name + " to its original configuration.");
    }

    void SetIncorrectIndicator(GameObject indicator, GameObject puzzlePiece)
    {
        RectTransform puzzleRect = puzzlePiece.GetComponent<RectTransform>();
        RectTransform indicatorRect = indicator.GetComponent<RectTransform>();

        indicatorRect.position = puzzleRect.position; // Set the indicator's position to match the puzzle piece
        indicatorRect.sizeDelta = puzzleRect.sizeDelta; // Set the indicator's size to match the puzzle piece
        indicator.SetActive(true); // Activate the indicator
    }

    bool IsPieceInCorrectSlot(GameObject piece, int index)
    {
        return piece.transform.parent.name == (index + 1).ToString() + "S";
    }
}
