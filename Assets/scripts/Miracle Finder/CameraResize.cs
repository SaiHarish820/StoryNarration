using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResize : MonoBehaviour
{
    private float defaultSize = 5; // Default size for a reference resolution
    private float targetAspect = 16f / 9f; // Target aspect ratio for 1920x1080 resolution

    // Start is called before the first frame update
    void Start()
    {
        UpdateCameraSize();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraSize();
    }

    private void UpdateCameraSize()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        GetComponent<Camera>().orthographicSize = defaultSize * (targetAspect / currentAspect);
    }
}
