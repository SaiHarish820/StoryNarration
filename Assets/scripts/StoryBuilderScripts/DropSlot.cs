using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour, IDropHandler
{
     /*public GameObject item
     {
         get
         {
             if (transform.childCount > 0)
             {
                 return transform.GetChild(0).gameObject;
             }
             return null;
         }
     }

     public void OnDrop(PointerEventData eventData)
     {
         Debug.Log("OnDrop triggered");
         if (!item)
         {
             DragHandler draggedItem = eventData.pointerDrag.GetComponent<DragHandler>();
             if (draggedItem)
             {
                 Debug.Log("Item Dropped on Slot: " + gameObject.name);
                 draggedItem.originalParent = transform; // Set the slot as the new parent
             }
             else
             {
                 Debug.Log("Dragged item does not have a DragHandler component");
             }
         }
         else
         {
             Debug.Log("Drop slot already has an item");
         }
     }*/
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag != gameObject)
        {
            // Handle the drop only if the dragged object is different from the drop target
            RectTransform droppedItem = eventData.pointerDrag.GetComponent<RectTransform>();
            droppedItem.SetParent(transform, false); // Adjust parent if necessary
                                                     // Additional handling code
            DragHandler draggedItem = eventData.pointerDrag.GetComponent<DragHandler>();
            if (draggedItem)
            {
                Debug.Log("Item Dropped on Slot: " + gameObject.name);
                draggedItem.originalParent = transform;// Set the slot as the new parent

                // Assuming 'draggedItem' has a RectTransform component
                RectTransform rectTransform = draggedItem.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    // Set the local position to zero
                    rectTransform.localPosition = Vector3.zero;
                    // Optionally reset local scale and local rotation if needed
                    rectTransform.localScale = Vector3.one;
                    rectTransform.localRotation = Quaternion.identity;

                    // Force the parent layout to update and correctly position the new child
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                }
                }


        }
        else
        {
            // Debug or handle cases where the drag starts and ends on the same object
            Debug.Log("Drag started and ended on the same object, drop not handled.");
            return;
        }
    }

    public void ForceLayoutUpdate()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

}
