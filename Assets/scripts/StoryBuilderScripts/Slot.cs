using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool isOccupied = false;  // Check if the slot is currently occupied
    public RectTransform rectTransform;  // Reference to the RectTransform of the slot

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Function to mark the slot as occupied
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
}
