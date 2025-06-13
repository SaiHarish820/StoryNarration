using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button playPauseButton;
    public Sprite playSprite;
    public Button backButton;
    public Sprite pauseSprite;
    public Slider seekBar;
    public Image clickableImage; // Image with collider to show/hide UI
    public Image seekBarHandleImage; // Image representing seek bar handle
    public string videoFileName; // Video file name stored in StreamingAssets

    private bool isDragging = false;
    private float uiVisibleTimer = 0f;
    private bool isUIVisible = false;

    void Start()
    {
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

        // Load and Play Video
        StartCoroutine(PlayVideo());
    }

    IEnumerator PlayVideo()
    {
        string videoPath;

#if UNITY_WEBGL
        videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
#else
            videoPath = "file://" + System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
#endif

        videoPlayer.url = videoPath;
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
    }

    void Update()
    {
        isDragging = Input.GetMouseButton(0) && RectTransformUtility.RectangleContainsScreenPoint(
            seekBarHandleImage.rectTransform, Input.mousePosition, null);

        if (!isDragging && videoPlayer.length > 0)
        {
            seekBar.value = (float)(videoPlayer.time / videoPlayer.length);
        }

        UpdateButtonSprite();

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
        SetUIVisibility(true);
        uiVisibleTimer = 5f;
    }

    void SetUIVisibility(bool visible)
    {
        isUIVisible = visible;
        backButton.gameObject.SetActive(visible);
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
        playPauseButton.image.sprite = videoPlayer.isPlaying ? pauseSprite : playSprite;
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

        string currentEpisode = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt(currentEpisode + "_Completed", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Level Page");
    }
}
