using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
   // private Transform originalPosition;
    public Transform originalPosition;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    public Transform originalParent; // To hold the original parent of the GameObject

    [SerializeField] private Canvas canvas;
    private RectTransform boundaryRectTransform;
    private Transform contentsTransform;
    public float snapDistance = 50f;
    private Vector2 originalSizeDelta;


    private void Start()
    {
        Debug.Log(originalPosition);
    }




    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        boundaryRectTransform = GameObject.Find("Content Slot").GetComponent<RectTransform>();
        contentsTransform = GameObject.Find("Contents").transform;
        originalSizeDelta = rectTransform.sizeDelta;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(canvas.transform);  // Ensure it's on top of other UI elements
        Debug.Log("Start Drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        DropSlot closestSlot = FindClosestDropSlot();
        if (closestSlot && Vector3.Distance(transform.position, closestSlot.transform.position) <= snapDistance)
        {
            rectTransform.sizeDelta = closestSlot.GetComponent<RectTransform>().sizeDelta;
        }
        else
        {
            rectTransform.sizeDelta = originalSizeDelta;
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
            transform.SetParent(closestSlot.transform, false);
            rectTransform.anchoredPosition = Vector2.zero; // Center in slot
            Debug.Log("Dropped within bounds and resized to match DropSlot.");
            closestSlot.isSlotFull = true; // Mark slot as full
        }
        else
        {
            // Revert to the original position and parent if drop is not valid
            transform.SetParent(originalParent);
            transform.position = originalPosition.position;
            Debug.Log(originalPosition);
            rectTransform.sizeDelta = originalSizeDelta; // Retain it's Original Size
            Debug.Log("Not in Boundary or too far from any DropSlot, or DropSlot is full, reset to original position");
        }
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
