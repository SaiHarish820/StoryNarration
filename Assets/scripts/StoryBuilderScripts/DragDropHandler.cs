using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Transform originalParent;    // The original parent of the GameObject
    public Transform snapTarget;        // Assign this via the Unity Inspector, pointing to the snap target
    private Vector3 startPosition;      // To store the position where the drag started

    private void Awake()
    {
        originalParent = transform.parent; // Initialize the originalParent
        if (!snapTarget) snapTarget = originalParent; // Default snapTarget to originalParent if none specified
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        startPosition = transform.position; // Store the start position
        transform.SetParent(transform.root); // Temporarily re-parent to root to avoid layout issues
        GetComponent<CanvasGroup>().blocksRaycasts = false; // Disable raycast to allow drop events
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        transform.position = Input.mousePosition; // Update position with the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        transform.SetParent(originalParent); // Reset the parent
        GetComponent<CanvasGroup>().blocksRaycasts = true; // Re-enable raycast

        // Snap to target position if no valid drop was handled (fallback)
        if (transform.parent == transform.root)
        {
            SnapToTarget();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop detected");
        eventData.pointerDrag.transform.SetParent(originalParent); // Re-parent the dragged object
        SnapToTarget(); // Snap the object to the designated snapTarget
    }

    private void SnapToTarget()
    {
        transform.position = snapTarget.position; // Move to the snap target's position
        transform.SetParent(originalParent); // Set to its original or new parent if it was changed during drag
    }
}
