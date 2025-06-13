using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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

    public bool dragEnabled = true; // Flag to enable or disable drag functionality



     private int numberofSlots = 9;

    // Audio variables
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource component
    [SerializeField] private AudioClip snapClip; // Sound when snapped into a valid slot
    [SerializeField] private AudioClip revertClip; // Sound when reverted to the original position

    

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        originalSizeDelta = rectTransform.sizeDelta;
        originalRotation = rectTransform.localRotation;

        string positionHolderName = $"{name}P";
        GameObject positionHolder = GameObject.Find(positionHolderName);
        if (positionHolder != null)
        {
            originalPosition = positionHolder.transform;
        }
        else
        {
            Debug.LogError($"Original Position GameObject '{positionHolderName}' not found for {gameObject.name}");
        }

        // Validate AudioSource setup
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!dragEnabled) return;

        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform); // Move to top canvas layer

        // Play the drag start sound

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragEnabled) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        AdjustSizeAndRotation();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragEnabled) return;

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        // Finish the drag and check if snapped or reverted
        FinishDrag();
    }

    void AdjustSizeAndRotation()
    {
        DropSlot closestSlot = FindClosestDropSlot();
        if (closestSlot && Vector3.Distance(transform.position, closestSlot.transform.position) <= snapDistance)
        {
            rectTransform.sizeDelta = closestSlot.GetComponent<RectTransform>().sizeDelta;
            rectTransform.localRotation = Quaternion.identity;
        }
        else
        {
            rectTransform.sizeDelta = originalSizeDelta;
            rectTransform.localRotation = Quaternion.identity;
        }
    }

    void FinishDrag()
    {
        DropSlot closestSlot = FindClosestDropSlot();
        if (closestSlot != null && Vector3.Distance(transform.position, closestSlot.transform.position) <= snapDistance && !closestSlot.isSlotFull)
        {
            transform.SetParent(closestSlot.transform, false);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localRotation = Quaternion.identity;

            // Play the snap sound
            PlaySound(snapClip);
        }
        else
        {
            // Revert to the original position if no valid slot was found
            transform.SetParent(originalParent);
            if (originalPosition != null)
            {
                transform.position = originalPosition.position;
            }
            rectTransform.sizeDelta = originalSizeDelta;
            rectTransform.localRotation = originalRotation;

            // Play the revert sound
            PlaySound(revertClip);
        }


    }

    DropSlot FindClosestDropSlot()
    {
        float closestDistance = float.MaxValue;
        DropSlot closestSlot = null;

        for (int i = 1; i <= numberofSlots; i++)
        {
            GameObject potentialSlotObject = GameObject.Find(i + "S");
            if (potentialSlotObject)
            {
                DropSlot slot = potentialSlotObject.GetComponent<DropSlot>();
                if (slot && !slot.isSlotFull)
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