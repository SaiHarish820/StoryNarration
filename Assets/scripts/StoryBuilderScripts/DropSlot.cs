using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public bool isSlotFull;
    
   // Tracks if the slot has an item

    void Update()
    {
        // Update the slot full status based on child count
        isSlotFull = transform.childCount > 0;
    }

    private void Start()
    {
        Debug.Log(isSlotFull);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Handle what happens when an item is dropped into this slot
        Debug.Log($"Item dropped into slot: {gameObject.name}");

        if (isSlotFull)
        {
            Debug.Log("Slot is already full. Handle accordingly.");
        }

    }
}
