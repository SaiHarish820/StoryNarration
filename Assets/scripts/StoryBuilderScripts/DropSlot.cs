using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public bool isSlotFull; // Tracks if the slot has an item

    void Update()
    {
        // Update the slot full status based on child count
        isSlotFull = transform.childCount > 0;
    }

    public void OnDrop(PointerEventData eventData)
    {
        /*if (eventData.pointerDrag != null && eventData.pointerDrag != gameObject)
        {
            if (isSlotFull)
            {
                Debug.Log("DropSlot is already full. Cannot drop another item.");
                // Invoke drag handler to reset position because the slot is full
                DragHandler dragHandler = eventData.pointerDrag.GetComponent<DragHandler>();
                if (dragHandler != null)
                {
                    eventData.pointerDrag.transform.SetParent(dragHandler.originalParent);  // Reset the parent
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = dragHandler.originalPosition;  // Reset the position
                }
                return; // Exit the method if the slot is full
            }

            // Logic to accept the drop if the slot is not full
            eventData.pointerDrag.transform.SetParent(transform, false);
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }*/
    }
}
