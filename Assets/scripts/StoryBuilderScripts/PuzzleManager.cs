using UnityEngine;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    public GameObject[] puzzlePieces; // Array to store the puzzle pieces
    public GameObject[] dropSlots; // Array to store the drop slots
    public GameObject[] incorrectIndicators; // Array to store the sprites that indicate an incorrect placement
    public Vector3[] originalPositions; // Array to store original positions of each puzzle piece
    public Vector2[] originalSizeDeltas; // Array to store original sizeDeltas (added for maintaining size)
    public Quaternion[] originalRotation; // Array to store original Rotation (added for maintaining rotation)
    public Transform originalParent; // Reference to the original parent for all pieces

    void Start()
    {
        originalPositions = new Vector3[puzzlePieces.Length];
        originalSizeDeltas = new Vector2[puzzlePieces.Length];
        originalRotation = new Quaternion[puzzlePieces.Length]; // Initialize the rotation array

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
        if (AllDropSlotsFilled())
        {
            CheckPuzzleCompletion();
        }
    }

    private bool AllDropSlotsFilled()
    {
        foreach (GameObject slot in dropSlots)
        {
            if (slot.transform.childCount == 0)
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
        }
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
