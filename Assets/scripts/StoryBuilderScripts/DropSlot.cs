using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour
{
    public bool isSlotFull;

    // Tracks if the slot has an item

    void Update()
    {
        // Update the slot full status based on child count
        isSlotFull = transform.childCount > 1;
    }

   

}