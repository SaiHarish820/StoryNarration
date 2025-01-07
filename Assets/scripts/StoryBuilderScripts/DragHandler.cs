using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    [SerializeField] private Transform originalParent; // Parent before drag starts
    private Transform originalPosition; // Position holder for reverting
    private Quaternion originalRotation; // Store original rotation

    [SerializeField] private Canvas canvas;
    public float snapDistance = 50f;
    private Vector2 originalSizeDelta;

    private bool isDraggable = true;
    private bool isLongPress = false;
    private float longPressThreshold = 3f; // Time in seconds
    private float pressStartTime;

    private DropSlot dropSlot;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        originalSizeDelta = rectTransform.sizeDelta;

        originalRotation = rectTransform.localRotation; // Store the initial rotation

        // Attempt to find DropSlot on this object
        dropSlot = GetComponent<DropSlot>();

        // Dynamically assign originalPosition based on naming convention
        string positionHolderName = $"{name}P"; // Example: "1" -> "1P"
        GameObject positionHolder = GameObject.Find(positionHolderName);

        if (positionHolder != null)
        {
            originalPosition = positionHolder.transform;
        }
        else
        {
            Debug.LogError($"Original Position GameObject '{positionHolderName}' not found for {gameObject.name}");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDraggable) return;

        pressStartTime = Time.time;

        if (dropSlot != null && dropSlot.isSlotFull)
        {
            Debug.Log("Pointer down on a full DropSlot");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        // Check if the DropSlot is full before allowing a long press drag
        if (dropSlot != null && dropSlot.isSlotFull)
        {
            // Detect long press only for filled slots
            isLongPress = Time.time - pressStartTime >= longPressThreshold;

            if (!isLongPress)
            {
                Debug.Log("Long press not detected; drag not allowed for filled slot.");
                return; // Exit if the long press condition is not met
            }

            Debug.Log("Long Press detected for a full DropSlot; starting drag.");
        }

        // Enable dragging
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform); // Move to top canvas layer

        Debug.Log(isLongPress ? "Long Press detected, starting drag." : "Normal drag started.");
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        DropSlot closestSlot = FindClosestDropSlot();
        if (closestSlot && Vector3.Distance(transform.position, closestSlot.transform.position) <= snapDistance)
        {
            rectTransform.sizeDelta = closestSlot.GetComponent<RectTransform>().sizeDelta;
            rectTransform.localRotation = Quaternion.identity; // Reset rotation
        }
        else
        {
            rectTransform.sizeDelta = originalSizeDelta;
            rectTransform.localRotation = originalRotation; // Restore original rotation
        }
        Debug.Log("Dragging");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        DropSlot closestSlot = FindClosestDropSlot();
        if (closestSlot != null && Vector3.Distance(transform.position, closestSlot.transform.position) <= snapDistance && !closestSlot.isSlotFull)
        {
            // Snap to the closest valid slot
            transform.SetParent(closestSlot.transform, false);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localRotation = Quaternion.identity; // Reset rotation
            closestSlot.isSlotFull = true;
            Debug.Log($"Dropped into slot: {closestSlot.name}");
        }
        else
        {
            // Revert to the original parent and position
            transform.SetParent(originalParent); // Restore original parent
            Debug.Log($"Reverting to original parent: {originalParent.name}");

            if (originalPosition != null)
            {
                transform.position = originalPosition.position; // Restore original position
                
            }
            rectTransform.sizeDelta = originalSizeDelta; // Restore original size
            
            Debug.Log($"Reverted to original position: {originalPosition?.position}");
        }

        isLongPress = false; // Reset state
    }

    private DropSlot FindClosestDropSlot()
    {
        float closestDistance = float.MaxValue;
        DropSlot closestSlot = null;

        // Iterate only over GameObjects with names that end with 'S' and start with a number, assuming they are named like '1S', '2S', etc.
        for (int i = 1; i <= 8; i++)
        {
            GameObject potentialSlotObject = GameObject.Find(i + "S");
            if (potentialSlotObject)
            {
                DropSlot slot = potentialSlotObject.GetComponent<DropSlot>();
                if (slot)
                {
                    float distance = Vector3.Distance(transform.position, slot.transform.position);
                    if (distance < closestDistance)
                    {
                        closestSlot = slot;
                        closestDistance = distance;
                    }
                }
            }
        }

        return closestSlot;
    }
}
