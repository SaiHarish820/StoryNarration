using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections; // Required for Coroutine

public class DragHandlerControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private DragHandler dragHandler;
    private float pressTimeThreshold = 0.2f;
    private float pressTimer;
    private bool isPressing;
    private bool canForceEnableDragHandler;

    [SerializeField] private Transform originalParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector2 originalSizeDelta;

    private RectTransform rectTransform;

    private readonly string[] validParentNames = { "1S", "2S", "3S", "4S", "5S", "6S", "7S", "8S", "9S" };

    private Vector3 originalScale;


    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource component
    [SerializeField] private AudioClip revertClip; // Sound when reverted to the original position

    void Awake()
    {
        dragHandler = GetComponent<DragHandler>();
        if (dragHandler == null)
        {
            Debug.LogError($"DragHandler component not found on {gameObject.name}");
        }

        originalScale = transform.localScale;

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform component is required for this script.");
        }

        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        originalSizeDelta = rectTransform.sizeDelta;

        // Validate AudioSource setup
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        bool isChildOfSpecificParent = IsChildOfSpecificParents();

        if (isChildOfSpecificParent && !canForceEnableDragHandler)
        {
            if (dragHandler.enabled)
            {
                dragHandler.enabled = false;
                Debug.Log("Disabling DragHandler as the object is under a valid parent.");
            }
        }
        else if (!isChildOfSpecificParent && !dragHandler.enabled)
        {
            dragHandler.enabled = true;
            Debug.Log("Enabling DragHandler as the object is not under a valid parent.");
        }

        if (isPressing)
        {
            pressTimer += Time.deltaTime;
            if (pressTimer >= pressTimeThreshold)
            {
                Debug.Log("Long press detected. Enabling DragHandler and enlarging GameObject.");

                canForceEnableDragHandler = true;
                dragHandler.enabled = true;
                transform.localScale = originalScale * 1.2f;

                // Call the coroutine to re-parent and restore position/size after .5 second
                StartCoroutine(DelayedReparentAndRestore());
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
        pressTimer = 0;
        transform.localScale = originalScale;
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

    /// <summary>
    /// Coroutine to delay re-parenting and restoring the original state.
    /// </summary>
    private IEnumerator DelayedReparentAndRestore()
    {
        yield return new WaitForSeconds(.5f); // Wait for .5 second

        // Ensure the object is re-parented to its original parent
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
        }

        // Restore position and size
        transform.localPosition = originalPosition;
        rectTransform.sizeDelta = originalSizeDelta;

        PlaySound(revertClip);

        Debug.Log("Re-parented and restored position/size after 1-second delay.");
    }

    /// <summary>
    /// Plays a sound effect using the AudioSource.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return; // Don't play if clip or AudioSource is missing
        audioSource.PlayOneShot(clip);
    }
}


