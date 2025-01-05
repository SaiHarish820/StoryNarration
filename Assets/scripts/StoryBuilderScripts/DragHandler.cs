using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalParent = transform.parent;
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;  // Set the transparency to 60% when dragging
        canvasGroup.blocksRaycasts = false;  // Allow events to pass through the dragged object
        transform.SetParent(transform.root);  // Move the object to the top of the UI hierarchy
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
        transform.SetParent(originalParent);  // Move back to its original parent in the hierarchy
        this.GetComponentInParent<HorizontalLayoutGroup>().enabled = true;
        Debug.Log("End Drag");

    }

  
}
