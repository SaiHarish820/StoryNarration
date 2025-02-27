using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button playPauseButton;
    public Sprite playSprite;
    public Sprite pauseSprite;
    public Slider seekBar;
    public Image clickableImage; // Image with collider to show/hide UI
    public Image seekBarHandleImage; // Image representing seek bar handle

    [SerializeField] private bool isDragging = false;
    private float uiVisibleTimer = 0f;
    private bool isUIVisible = false;

    void Start()
    {
        // Initialize UI visibility
        SetUIVisibility(false);

        // Add listeners
        playPauseButton.onClick.AddListener(TogglePlayPause);
        seekBar.onValueChanged.AddListener(OnSeekBarValueChanged);
        videoPlayer.loopPointReached += OnVideoEnd;

        // Add click listener to the image
        EventTrigger trigger = clickableImage.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => OnImageClick());
        trigger.triggers.Add(entry);

        // Set seek bar range
        seekBar.minValue = 0;
        seekBar.maxValue = 1;
    }

    void Update()
    {
        // Check if seek bar handle is being pressed
        isDragging = Input.GetMouseButton(0) && RectTransformUtility.RectangleContainsScreenPoint(
            seekBarHandleImage.rectTransform, Input.mousePosition, null);

        // Update seek bar if not dragging
        if (!isDragging && videoPlayer.length > 0)
        {
            seekBar.value = (float)(videoPlayer.time / videoPlayer.length);
        }

        // Update button sprite
        UpdateButtonSprite();

        // Handle UI visibility timer
        if (isUIVisible)
        {
            uiVisibleTimer -= Time.deltaTime;
            if (uiVisibleTimer <= 0)
            {
                SetUIVisibility(false);
            }
        }
    }

    void OnImageClick()
    {
        // Show UI and reset the timer
        SetUIVisibility(true);
        uiVisibleTimer = 5f; // 5 seconds
    }

    void SetUIVisibility(bool visible)
    {
        isUIVisible = visible;
        playPauseButton.gameObject.SetActive(visible);
        seekBar.gameObject.SetActive(visible);
    }

    void TogglePlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
        UpdateButtonSprite();
    }

    void UpdateButtonSprite()
    {
        if (videoPlayer.isPlaying)
        {
            playPauseButton.image.sprite = pauseSprite;
        }
        else
        {
            playPauseButton.image.sprite = playSprite;
        }
    }

    void OnSeekBarValueChanged(float value)
    {
        if (isDragging && videoPlayer.length > 0)
        {
            videoPlayer.time = value * videoPlayer.length;
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        seekBar.value = 0;
        UpdateButtonSprite();
        SceneManager.LoadScene("Level Page");
    }
}
