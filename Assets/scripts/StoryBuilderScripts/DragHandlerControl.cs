using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandlerControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private DragHandler dragHandler;
    private float pressTimeThreshold = 0.2f; // Threshold for long press detection
    private float pressTimer;
    private bool isPressing;
    private bool canForceEnableDragHandler; // Flag to manage drag handler state

    private readonly string[] validParentNames = { "1S", "2S", "3S", "4S", "5S", "6S", "7S", "8S" };

    private Vector3 originalScale; // To store the original scale of the GameObject

    void Awake()
    {
        dragHandler = GetComponent<DragHandler>();
        if (dragHandler == null)
        {
            Debug.LogError($"DragHandler component not found on {gameObject.name}");
        }

        originalScale = transform.localScale; // Store the original scale
    }

    void Update()
    {
        bool isChildOfSpecificParent = IsChildOfSpecificParents();

        if (isChildOfSpecificParent && !canForceEnableDragHandler)
        {
            if (dragHandler.enabled)
            {
                Debug.Log("Disabling DragHandler as object is under a valid parent.");
                dragHandler.enabled = false;
            }
        }
        else if (!isChildOfSpecificParent && !dragHandler.enabled)
        {
            Debug.Log("Enabling DragHandler as object is not under a valid parent.");
            dragHandler.enabled = true;
        }

        if (isPressing)
        {
            pressTimer += Time.deltaTime;
            if (pressTimer >= pressTimeThreshold)
            {
                Debug.Log("Long press detected. Enabling DragHandler and enlarging GameObject.");
                canForceEnableDragHandler = true;
                dragHandler.enabled = true;
                transform.localScale = originalScale * 1.2f; // Enlarge by 20%
                isPressing = false;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsChildOfSpecificParents())
        {
            isPressing = true;
            pressTimer = 0;
            Debug.Log("Start pressing for long press detection.");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressing = false;
        pressTimer = 0; // Reset timer
        transform.localScale = originalScale; // Reset to original scale
        Debug.Log("Stopped pressing. Resetting scale.");
    }

    private bool IsChildOfSpecificParents()
    {
        var parent = transform.parent;
        if (parent == null) return false;

        foreach (string parentName in validParentNames)
        {
            if (parent.name == parentName)
            {
                return true;
            }
        }

        return false;
    }

    private void OnTransformParentChanged()
    {
        if (IsChildOfSpecificParents())
        {
            Debug.Log("Parent changed to a valid parent. Disabling DragHandler.");
            canForceEnableDragHandler = false;
            dragHandler.enabled = false;
        }
        else
        {
            Debug.Log("Parent changed to an invalid parent. Enabling DragHandler.");
            dragHandler.enabled = true;
        }
    }
}
