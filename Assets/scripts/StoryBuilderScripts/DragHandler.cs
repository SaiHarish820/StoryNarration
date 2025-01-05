using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
   [HideInInspector] public Transform parentAfterDrag;
    public Transform originalPosition;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    private RectTransform boundaryRectTransform;
    private Transform contentsTransform;// RectTransform of the boundary object

    // Declare the list to hold the ContentPosition GameObjects
    public Transform[] contentPositions;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        // Initialize the boundary RectTransform; adjust to refer to your specific boundary object
        boundaryRectTransform = GameObject.Find("Content Slot").GetComponent<RectTransform>();

        // Find and store the transform of the "Contents" GameObject
        contentsTransform = GameObject.Find("Contents").transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;  // Set the transparency to 60% when dragging
        canvasGroup.blocksRaycasts = false;  // Allow events to pass through the dragged object
        parentAfterDrag = transform.parent;  // Move the object to the top of the UI hierarchy
        transform.SetParent(transform.root);  
        transform.SetAsLastSibling();
        this.GetComponentInParent<HorizontalLayoutGroup>().enabled = false;
        Debug.Log("Start Drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        Debug.Log("Dragging");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;  // Restore transparency to 100%
        canvasGroup.blocksRaycasts = true;  // Ensure this object can be interacted with again
        transform.SetParent(parentAfterDrag);  // Move back to its original parent in the hierarchy
        this.GetComponentInParent<HorizontalLayoutGroup>().enabled = true;
        Debug.Log("End Drag");

        if (!IsWithinBounds())
        {
            // Break the parent-child relationship
            transform.SetParent(null);

            // Attach the GameObject to the "Contents" GameObject as its new parent
            transform.SetParent(contentsTransform, false);
            // Reset local position and scale to default or desired values
            RectTransform rectTransform = GetComponent<RectTransform>();
            //rectTransform.anchoredPosition = Vector2.zero;  // Position it at the center of the new parent
            rectTransform.localScale = Vector3.one;          // Reset scale to 1

             // Optionally, move the item back to its original position or a "safe" area
             transform.position = originalPosition.position;
            Debug.Log("Not in Boundary");
           
           
        }

    }

    private void Update()
    {
        IsWithinBounds();
    }

    private bool IsWithinBounds()
    {
        // Convert the item's current world position to a local position within the boundary's RectTransform space
        Vector3 localPos = boundaryRectTransform.InverseTransformPoint(transform.position);
        // Check if this local position is within the boundary's rect
        return boundaryRectTransform.rect.Contains(localPos);
    }


}
